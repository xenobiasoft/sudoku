namespace Sudoku.Domain.Entities;

public class SudokuGame : AggregateRoot
{
    private readonly List<Cell> _cells;
    private readonly List<MoveHistory> _moveHistory;

    public GameId Id { get; private set; }
    public PlayerAlias PlayerAlias { get; private set; }
    public GameDifficulty Difficulty { get; private set; }
    public GameStatus Status { get; private set; }
    public GameStatistics Statistics { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? PausedAt { get; private set; }

    private SudokuGame()
    {
        _cells = [];
        _moveHistory = [];
    }

    public static SudokuGame Create(PlayerAlias playerAlias, GameDifficulty difficulty, IEnumerable<Cell> initialCells)
    {
        var game = new SudokuGame
        {
            Id = GameId.New(),
            PlayerAlias = playerAlias,
            Difficulty = difficulty,
            Status = GameStatus.NotStarted,
            Statistics = GameStatistics.Create(),
            CreatedAt = DateTime.UtcNow
        };

        game._cells.AddRange(initialCells);

        game.AddDomainEvent(new GameCreatedEvent(game.Id, playerAlias, difficulty));
        return game;
    }

    public void StartGame()
    {
        if (Status != GameStatus.NotStarted)
        {
            throw new GameNotInStartStateException($"Cannot start game in {Status} state");
        }

        Status = GameStatus.InProgress;
        StartedAt = DateTime.UtcNow;

        AddDomainEvent(new GameStartedEvent(Id));
    }

    public void MakeMove(int row, int column, int? value)
    {
        if (Status != GameStatus.InProgress)
        {
            throw new GameNotInProgressException($"Cannot make move in {Status} state");
        }

        var cell = GetCell(row, column);
        if (cell.IsFixed)
        {
            throw new CellIsFixedException($"Cannot modify fixed cell at position ({row}, {column})");
        }

        // Only validate move if we're setting a value (not clearing)
        if (value.HasValue && !IsValidMove(row, column, value.Value))
        {
            throw new InvalidMoveException($"Invalid move: {value} at position ({row}, {column})");
        }

        // Record move history before making the change
        var previousValue = cell.Value;
        _moveHistory.Add(new MoveHistory(row, column, previousValue, value));

        cell.SetValue(value);
        Statistics.RecordMove(true);

        AddDomainEvent(new MoveMadeEvent(Id, row, column, value, Statistics));

        if (IsGameComplete())
        {
            CompleteGame();
        }
    }

    public void UndoLastMove()
    {
        if (Status != GameStatus.InProgress)
        {
            throw new GameNotInProgressException($"Cannot undo move in {Status} state");
        }

        if (_moveHistory.Count == 0)
        {
            throw new NoMoveHistoryException("No moves to undo");
        }

        var lastMove = _moveHistory[^1];
        _moveHistory.RemoveAt(_moveHistory.Count - 1);

        var cell = GetCell(lastMove.Row, lastMove.Column);
        cell.SetValue(lastMove.PreviousValue);

        // Decrement the move count in statistics
        Statistics.UndoMove();

        AddDomainEvent(new MoveUndoneEvent(Id, lastMove.Row, lastMove.Column, lastMove.PreviousValue));
    }

    public void ResetGame()
    {
        if (Status == GameStatus.NotStarted)
        {
            throw new GameNotInStartStateException("Game is already in its initial state");
        }

        // Reset all non-fixed cells
        foreach (var cell in _cells.Where(c => !c.IsFixed))
        {
            cell.SetValue(null);
        }

        // Clear move history
        _moveHistory.Clear();

        // Reset statistics but keep the original timestamp
        Statistics = GameStatistics.Create();

        // Set the game state back to InProgress if it was completed or abandoned
        if (Status == GameStatus.Completed || Status == GameStatus.Abandoned)
        {
            Status = GameStatus.InProgress;
            CompletedAt = null;
        }

        AddDomainEvent(new GameResetEvent(Id));
    }

    public ValidationResult ValidateGame()
    {
        var errors = new List<string>();
        var isValid = true;

        // Validate rows
        for (int row = 0; row < 9; row++)
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
        for (int column = 0; column < 9; column++)
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

        // Validate 3x3 boxes
        for (int boxRow = 0; boxRow < 9; boxRow += 3)
        {
            for (int boxColumn = 0; boxColumn < 9; boxColumn += 3)
            {
                var boxValues = _cells.Where(c => c.Row >= boxRow && c.Row < boxRow + 3 &&
                                                 c.Column >= boxColumn && c.Column < boxColumn + 3 &&
                                                 c.HasValue)
                                     .Select(c => c.Value!.Value)
                                     .ToList();
                
                if (boxValues.Count != boxValues.Distinct().Count())
                {
                    errors.Add($"Box at position ({boxRow / 3 + 1}, {boxColumn / 3 + 1}) contains duplicate values");
                    isValid = false;
                }
            }
        }

        return new ValidationResult(isValid, errors);
    }

    public void PauseGame()
    {
        if (Status != GameStatus.InProgress)
        {
            throw new GameNotInProgressException($"Cannot pause game in {Status} state");
        }

        Status = GameStatus.Paused;
        PausedAt = DateTime.UtcNow;

        AddDomainEvent(new GamePausedEvent(Id));
    }

    public void ResumeGame()
    {
        if (Status != GameStatus.Paused)
        {
            throw new GameNotPausedException($"Cannot resume game in {Status} state");
        }

        Status = GameStatus.InProgress;
        PausedAt = null;

        AddDomainEvent(new GameResumedEvent(Id));
    }

    public void AbandonGame()
    {
        if (Status == GameStatus.Completed)
        {
            throw new GameAlreadyCompletedException("Cannot abandon completed game");
        }

        Status = GameStatus.Abandoned;

        AddDomainEvent(new GameAbandonedEvent(Id));
    }

    public void UpdatePlayDuration(TimeSpan duration)
    {
        Statistics.UpdatePlayDuration(duration);
    }

    public IReadOnlyList<Cell> GetCells() => _cells.AsReadOnly();

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
        var boxRow = row / 3 * 3;
        var boxColumn = column / 3 * 3;

        return _cells.Where(c => c.Row >= boxRow && c.Row < boxRow + 3 && c.Column >= boxColumn && c.Column < boxColumn + 3 && c.HasValue).All(c => c.Value != value);
    }

    private bool IsValidPuzzle()
    {
        for (int row = 0; row < 9; row++)
        {
            var rowValues = _cells.Where(c => c.Row == row && c.HasValue)
                                 .Select(c => c.Value!.Value)
                                 .ToList();
            if (rowValues.Count != 9 || rowValues.Distinct().Count() != 9)
            {
                return false;
            }
        }

        for (int column = 0; column < 9; column++)
        {
            var columnValues = _cells.Where(c => c.Column == column && c.HasValue)
                                    .Select(c => c.Value!.Value)
                                    .ToList();
            if (columnValues.Count != 9 || columnValues.Distinct().Count() != 9)
            {
                return false;
            }
        }

        for (int boxRow = 0; boxRow < 9; boxRow += 3)
        {
            for (int boxColumn = 0; boxColumn < 9; boxColumn += 3)
            {
                var boxValues = _cells.Where(c => c.Row >= boxRow && c.Row < boxRow + 3 &&
                                                 c.Column >= boxColumn && c.Column < boxColumn + 3 &&
                                                 c.HasValue)
                                     .Select(c => c.Value!.Value)
                                     .ToList();
                if (boxValues.Count != 9 || boxValues.Distinct().Count() != 9)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void CompleteGame()
    {
        Status = GameStatus.Completed;
        CompletedAt = DateTime.UtcNow;

        AddDomainEvent(new GameCompletedEvent(Id, Statistics));
    }
}

public record MoveHistory(int Row, int Column, int? PreviousValue, int? NewValue);

public record ValidationResult(bool IsValid, List<string> Errors);