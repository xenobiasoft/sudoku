namespace Sudoku.Domain.Entities;

public class SudokuGame : AggregateRoot
{
    private readonly List<Cell> _cells;
    private readonly List<GameHistoryEntry> _history;

    public GameId Id { get; private set; }
    public ProfileId ProfileId { get; private set; }
    public PlayerAlias DisplayName { get; private set; }
    public GameDifficulty Difficulty { get; private set; }
    public BoardSize Size { get; private set; }
    public GameStatusEnum Status { get; private set; }
    public GameStatistics Statistics { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? PausedAt { get; private set; }
    public IReadOnlyList<MoveHistoryEntry> MoveHistory => _history.OfType<MoveHistoryEntry>().ToList();

    private SudokuGame()
    {
        _cells = [];
        _history = [];
    }

    public static SudokuGame Create(ProfileId profileId, PlayerAlias displayName, GameDifficulty difficulty, BoardSize size, IEnumerable<Cell> initialCells)
    {
        var game = new SudokuGame
        {
            Id = GameId.New(),
            ProfileId = profileId,
            DisplayName = displayName,
            Difficulty = difficulty,
            Size = size,
            Status = GameStatusEnum.NotStarted,
            Statistics = GameStatistics.Create(),
            CreatedAt = DateTime.UtcNow
        };

        game._cells.AddRange(initialCells);

        game.AddDomainEvent(new GameCreatedEvent(game.Id, profileId, displayName, difficulty, size));
        return game;
    }

    public static SudokuGame Reconstitute(
        GameId id,
        ProfileId profileId,
        PlayerAlias displayName,
        GameDifficulty difficulty,
        BoardSize size,
        GameStatusEnum statusEnum,
        GameStatistics statistics,
        IEnumerable<Cell> cells,
        IEnumerable<GameHistoryEntry> history,
        DateTime createdAt,
        DateTime? startedAt,
        DateTime? completedAt,
        DateTime? pausedAt)
    {
        var game = new SudokuGame
        {
            Id = id,
            ProfileId = profileId,
            DisplayName = displayName,
            Difficulty = difficulty,
            Size = size,
            Status = statusEnum,
            Statistics = statistics,
            CreatedAt = createdAt,
            StartedAt = startedAt,
            CompletedAt = completedAt,
            PausedAt = pausedAt
        };

        game._cells.AddRange(cells);
        game._history.AddRange(history);

        return game;
    }

    public void StartGame()
    {
        if (Status != GameStatusEnum.NotStarted)
        {
            throw new GameNotInStartStateException($"Cannot start game in {Status} state");
        }

        Status = GameStatusEnum.InProgress;
        StartedAt = DateTime.UtcNow;

        AddDomainEvent(new GameStartedEvent(Id));
    }

    public void MakeMove(int row, int column, int? value)
    {
        if (Status != GameStatusEnum.InProgress)
        {
            throw new GameNotInProgressException($"Cannot make move in {Status} state");
        }

        var cell = GetCell(row, column);
        if (cell.IsLocked)
        {
            throw new CellIsFixedException($"Cannot modify fixed cell at position ({row}, {column})");
        }

        // Check if this is a valid move (only when setting a value, not clearing)
        var isValid = !value.HasValue || IsValidMove(row, column, value.Value);

        var previousValue = cell.Value;
        cell.SetValue(value);
        Statistics.RecordMove(isValid);

        var peerEliminations = value.HasValue
            ? EliminatePeerCandidates(row, column, value.Value)
            : [];

        _history.Add(new MoveHistoryEntry(row, column, previousValue, value, peerEliminations));

        AddDomainEvent(new MoveMadeEvent(Id, row, column, value, Statistics));

        foreach (var elimination in peerEliminations)
        {
            AddDomainEvent(new PossibleValueRemovedEvent(Id, elimination.Row, elimination.Column, elimination.Value));
        }

        if (IsGameComplete())
        {
            CompleteGame();
        }
    }

    private List<PeerElimination> EliminatePeerCandidates(int row, int column, int value)
    {
        var boxRow = row / Size.BoxSize * Size.BoxSize;
        var boxColumn = column / Size.BoxSize * Size.BoxSize;

        var peers = _cells.Where(c =>
            (c.Row == row || c.Column == column ||
             (c.Row >= boxRow && c.Row < boxRow + Size.BoxSize && c.Column >= boxColumn && c.Column < boxColumn + Size.BoxSize)) &&
            !(c.Row == row && c.Column == column));

        var eliminations = new List<PeerElimination>();
        foreach (var peer in peers)
        {
            if (peer.PossibleValues.Contains(value))
            {
                peer.RemovePossibleValue(value);
                eliminations.Add(new PeerElimination(peer.Row, peer.Column, value));
            }
        }

        return eliminations;
    }

    /// <summary>
    /// Reveals the correct value for a randomly chosen empty cell, locking it in place.
    /// The caller supplies the solved puzzle (computed from the fixed clues), so the domain
    /// stays free of any solver dependency. Hints do not enter the move history, so they are
    /// unaffected by <see cref="UndoLastMove"/>.
    /// </summary>
    public (int Row, int Column, int Value) RevealHint(SudokuPuzzle solvedPuzzle)
    {
        if (Status != GameStatusEnum.InProgress)
        {
            throw new GameNotInProgressException($"Cannot reveal a hint in {Status} state");
        }

        if (Statistics.HintsUsed >= Size.MaxHints)
        {
            throw new HintLimitReachedException();
        }

        var emptyCells = _cells.Where(c => !c.IsLocked && !c.HasValue).ToList();
        if (emptyCells.Count == 0)
        {
            throw new NoAvailableCellsForHintException();
        }

        var target = emptyCells[Random.Shared.Next(emptyCells.Count)];
        var correctValue = solvedPuzzle.GetCell(target.Row, target.Column).Value
            ?? throw new NoAvailableCellsForHintException();

        var index = _cells.FindIndex(c => c.Row == target.Row && c.Column == target.Column);
        _cells[index] = Cell.CreateHint(target.Row, target.Column, correctValue, Size);

        var peerEliminations = EliminatePeerCandidates(target.Row, target.Column, correctValue);

        Statistics.RecordHint();

        AddDomainEvent(new HintRevealedEvent(Id, target.Row, target.Column, correctValue, Statistics));

        foreach (var elimination in peerEliminations)
        {
            AddDomainEvent(new PossibleValueRemovedEvent(Id, elimination.Row, elimination.Column, elimination.Value));
        }

        if (IsGameComplete())
        {
            CompleteGame();
        }

        return (target.Row, target.Column, correctValue);
    }

    public void AddPossibleValue(int row, int column, int value)
    {
        if (Status != GameStatusEnum.InProgress)
        {
            throw new GameNotInProgressException($"Cannot add possible value in {Status} state");
        }

        var cell = GetCell(row, column);
        if (cell.IsFixed)
        {
            throw new CellIsFixedException($"Cannot modify fixed cell at position ({row}, {column})");
        }

        if (cell.HasValue)
        {
            throw new CellAlreadyHasValueException($"Cannot add possible values to cell with a definite value at position ({row}, {column})");
        }

        cell.AddPossibleValue(value);
        _history.Add(new PossibleValueAddedEntry(row, column, value));
        AddDomainEvent(new PossibleValueAddedEvent(Id, row, column, value));
    }

    public void RemovePossibleValue(int row, int column, int value)
    {
        if (Status != GameStatusEnum.InProgress)
        {
            throw new GameNotInProgressException($"Cannot remove possible value in {Status} state");
        }

        var cell = GetCell(row, column);
        if (cell.IsFixed)
        {
            throw new CellIsFixedException($"Cannot modify fixed cell at position ({row}, {column})");
        }

        cell.RemovePossibleValue(value);
        _history.Add(new PossibleValueRemovedEntry(row, column, value));
        AddDomainEvent(new PossibleValueRemovedEvent(Id, row, column, value));
    }

    public void ClearPossibleValues(int row, int column)
    {
        if (Status != GameStatusEnum.InProgress)
        {
            throw new GameNotInProgressException($"Cannot clear possible values in {Status} state");
        }

        var cell = GetCell(row, column);
        if (cell.IsFixed)
        {
            throw new CellIsFixedException($"Cannot modify fixed cell at position ({row}, {column})");
        }

        var previousValues = cell.PossibleValues.ToList();
        cell.ClearPossibleValues();
        _history.Add(new PossibleValuesClearedEntry(row, column, previousValues));
        AddDomainEvent(new PossibleValuesClearedEvent(Id, row, column));
    }

    public void UndoLastMove()
    {
        if (Status != GameStatusEnum.InProgress)
        {
            throw new GameNotInProgressException($"Cannot undo move in {Status} state");
        }

        if (_history.Count == 0)
        {
            throw new NoMoveHistoryException();
        }

        var lastEntry = _history[^1];
        _history.RemoveAt(_history.Count - 1);

        switch (lastEntry)
        {
            case MoveHistoryEntry move:
                var cell = GetCell(move.Row, move.Column);
                cell.SetValue(move.PreviousValue);

                foreach (var elimination in move.PeerEliminations)
                {
                    GetCell(elimination.Row, elimination.Column).AddPossibleValue(elimination.Value);
                }

                Statistics.UndoMove();
                AddDomainEvent(new MoveUndoneEvent(Id, move.Row, move.Column, move.PreviousValue));
                break;

            case PossibleValueAddedEntry added:
                GetCell(added.Row, added.Column).RemovePossibleValue(added.Value);
                AddDomainEvent(new PossibleValueRemovedEvent(Id, added.Row, added.Column, added.Value));
                break;

            case PossibleValueRemovedEntry removed:
                GetCell(removed.Row, removed.Column).AddPossibleValue(removed.Value);
                AddDomainEvent(new PossibleValueAddedEvent(Id, removed.Row, removed.Column, removed.Value));
                break;

            case PossibleValuesClearedEntry cleared:
                var clearedCell = GetCell(cleared.Row, cleared.Column);
                foreach (var previousValue in cleared.PreviousValues)
                {
                    clearedCell.AddPossibleValue(previousValue);
                }
                break;
        }
    }

    public void ResetGame()
    {
        if (Status == GameStatusEnum.NotStarted)
        {
            throw new GameNotInStartStateException("Game is already in its initial state");
        }

        // Reset all non-fixed cells; hint-revealed cells are locked, so rebuild them as empty
        for (var i = 0; i < _cells.Count; i++)
        {
            var cell = _cells[i];
            if (cell.IsFixed)
            {
                continue;
            }

            if (cell.IsHint)
            {
                _cells[i] = Cell.CreateEmpty(cell.Row, cell.Column, Size);
            }
            else
            {
                cell.SetValue(null);
                cell.ClearPossibleValues();
            }
        }

        // Clear move history
        _history.Clear();

        // Reset statistics but keep the original timestamp
        Statistics = GameStatistics.Create();

        // Set the game state back to InProgress if it was completed or abandoned
        if (Status == GameStatusEnum.Completed || Status == GameStatusEnum.Abandoned)
        {
            Status = GameStatusEnum.InProgress;
            CompletedAt = null;
        }

        AddDomainEvent(new GameResetEvent(Id));
    }

    public ValidationResult ValidateGame()
    {
        var errors = new List<string>();
        var isValid = true;

        // Validate rows
        for (int row = 0; row < Size.Size; row++)
        {
            var rowValues = _cells.Where(c => c.Row == row && c.HasValue)
                                 .Select(c => c.Value!.Value)
                                 .ToList();

            if (rowValues.Count != rowValues.Distinct().Count())
            {
                errors.Add($"Row {row + 1} contains duplicate values");
                isValid = false;
            }
        }

        // Validate columns
        for (int column = 0; column < Size.Size; column++)
        {
            var columnValues = _cells.Where(c => c.Column == column && c.HasValue)
                                    .Select(c => c.Value!.Value)
                                    .ToList();

            if (columnValues.Count != columnValues.Distinct().Count())
            {
                errors.Add($"Column {column + 1} contains duplicate values");
                isValid = false;
            }
        }

        // Validate boxes
        for (int boxRow = 0; boxRow < Size.Size; boxRow += Size.BoxSize)
        {
            for (int boxColumn = 0; boxColumn < Size.Size; boxColumn += Size.BoxSize)
            {
                var boxValues = _cells.Where(c => c.Row >= boxRow && c.Row < boxRow + Size.BoxSize &&
                                                 c.Column >= boxColumn && c.Column < boxColumn + Size.BoxSize &&
                                                 c.HasValue)
                                     .Select(c => c.Value!.Value)
                                     .ToList();

                if (boxValues.Count != boxValues.Distinct().Count())
                {
                    errors.Add($"Box at position ({boxRow / Size.BoxSize + 1}, {boxColumn / Size.BoxSize + 1}) contains duplicate values");
                    isValid = false;
                }
            }
        }

        return new ValidationResult(isValid, errors);
    }

    public void PauseGame()
    {
        if (Status != GameStatusEnum.InProgress)
        {
            throw new GameNotInProgressException($"Cannot pause game in {Status} state");
        }

        Status = GameStatusEnum.Paused;
        PausedAt = DateTime.UtcNow;

        AddDomainEvent(new GamePausedEvent(Id));
    }

    public void ResumeGame()
    {
        if (Status != GameStatusEnum.Paused && Status != GameStatusEnum.NotStarted)
        {
            throw new GameNotPausedException($"Cannot resume game in {Status} state. Game must be NotStarted or Paused.");
        }

        // If starting from NotStarted, set the StartedAt timestamp
        if (Status == GameStatusEnum.NotStarted)
        {
            StartedAt = DateTime.UtcNow;
        }

        Status = GameStatusEnum.InProgress;
        PausedAt = null;

        AddDomainEvent(new GameResumedEvent(Id));
    }

    public void AbandonGame()
    {
        if (Status == GameStatusEnum.Completed)
        {
            throw new GameAlreadyCompletedException("Cannot abandon completed game");
        }

        Status = GameStatusEnum.Abandoned;

        AddDomainEvent(new GameAbandonedEvent(Id));
    }

    public void UpdatePlayDuration(TimeSpan duration)
    {
        Statistics.UpdatePlayDuration(duration);
    }

    public IReadOnlyList<Cell> GetCells() => _cells.AsReadOnly();

    public IReadOnlyList<GameHistoryEntry> GetHistory() => _history.AsReadOnly();

    public Cell GetCell(int row, int column)
    {
        return _cells.FirstOrDefault(c => c.Row == row && c.Column == column) ?? throw new CellNotFoundException($"Cell not found at position ({row}, {column})");
    }

    public bool IsGameComplete()
    {
        return _cells.All(c => c.HasValue) && IsValidPuzzle();
    }

    private bool IsValidMove(int row, int column, int value)
    {
        return IsValidForRow(row, value) &&
               IsValidForColumn(column, value) &&
               IsValidForBox(row, column, value);
    }

    private bool IsValidForRow(int row, int value)
    {
        return _cells.Where(c => c.Row == row && c.HasValue).All(c => c.Value != value);
    }

    private bool IsValidForColumn(int column, int value)
    {
        return _cells.Where(c => c.Column == column && c.HasValue).All(c => c.Value != value);
    }

    private bool IsValidForBox(int row, int column, int value)
    {
        var boxRow = row / Size.BoxSize * Size.BoxSize;
        var boxColumn = column / Size.BoxSize * Size.BoxSize;

        return _cells.Where(c => c.Row >= boxRow && c.Row < boxRow + Size.BoxSize && c.Column >= boxColumn && c.Column < boxColumn + Size.BoxSize && c.HasValue).All(c => c.Value != value);
    }

    private bool IsValidPuzzle()
    {
        for (int row = 0; row < Size.Size; row++)
        {
            var rowValues = _cells.Where(c => c.Row == row && c.HasValue)
                                 .Select(c => c.Value!.Value)
                                 .ToList();
            if (rowValues.Count != Size.Size || rowValues.Distinct().Count() != Size.Size)
            {
                return false;
            }
        }

        for (int column = 0; column < Size.Size; column++)
        {
            var columnValues = _cells.Where(c => c.Column == column && c.HasValue)
                                    .Select(c => c.Value!.Value)
                                    .ToList();
            if (columnValues.Count != Size.Size || columnValues.Distinct().Count() != Size.Size)
            {
                return false;
            }
        }

        for (int boxRow = 0; boxRow < Size.Size; boxRow += Size.BoxSize)
        {
            for (int boxColumn = 0; boxColumn < Size.Size; boxColumn += Size.BoxSize)
            {
                var boxValues = _cells.Where(c => c.Row >= boxRow && c.Row < boxRow + Size.BoxSize &&
                                                 c.Column >= boxColumn && c.Column < boxColumn + Size.BoxSize &&
                                                 c.HasValue)
                                     .Select(c => c.Value!.Value)
                                     .ToList();
                if (boxValues.Count != Size.Size || boxValues.Distinct().Count() != Size.Size)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void CompleteGame()
    {
        var completedAt = DateTime.UtcNow;

        Status = GameStatusEnum.Completed;
        CompletedAt = completedAt;

        AddDomainEvent(new GameCompletedEvent(Id, ProfileId, Difficulty, Statistics, completedAt, Size));
    }
}

public abstract record GameHistoryEntry(int Row, int Column);

public sealed record PeerElimination(int Row, int Column, int Value);

public sealed record MoveHistoryEntry(
    int Row,
    int Column,
    int? PreviousValue,
    int? NewValue,
    IReadOnlyList<PeerElimination> PeerEliminations) : GameHistoryEntry(Row, Column);

public sealed record PossibleValueAddedEntry(int Row, int Column, int Value) : GameHistoryEntry(Row, Column);

public sealed record PossibleValueRemovedEntry(int Row, int Column, int Value) : GameHistoryEntry(Row, Column);

public sealed record PossibleValuesClearedEntry(int Row, int Column, IReadOnlyList<int> PreviousValues) : GameHistoryEntry(Row, Column);

public record ValidationResult(bool IsValid, List<string> Errors);
