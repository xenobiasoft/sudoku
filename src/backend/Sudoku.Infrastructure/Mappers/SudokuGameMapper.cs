using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Models;

namespace Sudoku.Infrastructure.Mappers;

public static class SudokuGameMapper
{
    public static SudokuGameDocument ToDocument(SudokuGame game)
    {
        var sudokuGameDocument = new SudokuGameDocument
        {
            Id = game.Id.Value.ToString(),
            GameId = game.Id.Value.ToString(),
            ProfileId = game.ProfileId.ToString(),
            DisplayName = game.DisplayName.Value,
            Difficulty = game.Difficulty.Name,
            GridSize = game.Size.Size,
            Status = game.Status,
            Cells = game.GetCells().Select(ToDocument).ToList(),
            Statistics = ToDocument(game.Statistics),
            History = game.GetHistory().Select(ToDocument).ToList(),
            CreatedAt = game.CreatedAt,
            StartedAt = game.StartedAt,
            CompletedAt = game.CompletedAt,
            PausedAt = game.PausedAt
        };
        return sudokuGameDocument;
    }

    public static SudokuGame ToDomain(SudokuGameDocument document)
    {
        var profileIdStr = string.IsNullOrEmpty(document.ProfileId) ? Guid.Empty.ToString() : document.ProfileId;
        var profileId = Guid.TryParse(profileIdStr, out var guid)
            ? ProfileId.From(guid)
            : ProfileId.From(Guid.Empty);

        var displayName = PlayerAlias.Create(string.IsNullOrEmpty(document.DisplayName) ? "Unknown" : document.DisplayName);

        var difficulty = string.IsNullOrEmpty(document.Difficulty)
            ? GameDifficulty.Easy
            : GameDifficulty.FromName(document.Difficulty);

        var size = BoardSize.FromValue(document.GridSize);

        if (document.Cells.Count != size.CellCount)
        {
            throw new InvalidPuzzleException();
        }

        var history = document.History.Count > 0
            ? document.History.Select(ToDomain)
            : document.MoveHistory.Select(m =>
                (GameHistoryEntry)new MoveHistoryEntry(m.Row, m.Column, m.PreviousValue, m.NewValue, []));

        var sudokuGame = SudokuGame.Reconstitute(
            GameId.Create(document.GameId),
            profileId,
            displayName,
            difficulty,
            size,
            document.Status,
            document.Statistics.ToDomain(),
            document.Cells.Select(c => c.ToDomain(size)).ToList(),
            history,
            document.CreatedAt,
            document.StartedAt,
            document.CompletedAt,
            document.PausedAt
        );

        return sudokuGame;
    }

    private static GameHistoryEntryDocument ToDocument(GameHistoryEntry entry) => entry switch
    {
        MoveHistoryEntry move => new GameHistoryEntryDocument
        {
            Type = "Move",
            Row = move.Row,
            Column = move.Column,
            PreviousValue = move.PreviousValue,
            NewValue = move.NewValue,
            PeerEliminations = move.PeerEliminations
                .Select(p => new PeerEliminationDocument { Row = p.Row, Column = p.Column, Value = p.Value })
                .ToList()
        },
        PossibleValueAddedEntry added => new GameHistoryEntryDocument
        {
            Type = "Added",
            Row = added.Row,
            Column = added.Column,
            Value = added.Value
        },
        PossibleValueRemovedEntry removed => new GameHistoryEntryDocument
        {
            Type = "Removed",
            Row = removed.Row,
            Column = removed.Column,
            Value = removed.Value
        },
        PossibleValuesClearedEntry cleared => new GameHistoryEntryDocument
        {
            Type = "Cleared",
            Row = cleared.Row,
            Column = cleared.Column,
            PreviousValues = cleared.PreviousValues.ToList()
        },
        _ => throw new InvalidOperationException($"Unknown history entry type: {entry.GetType().Name}")
    };

    private static GameHistoryEntry ToDomain(GameHistoryEntryDocument document) => document.Type switch
    {
        "Move" => new MoveHistoryEntry(
            document.Row,
            document.Column,
            document.PreviousValue,
            document.NewValue,
            document.PeerEliminations.Select(p => new PeerElimination(p.Row, p.Column, p.Value)).ToList()),
        "Added" => new PossibleValueAddedEntry(document.Row, document.Column, document.Value ?? 0),
        "Removed" => new PossibleValueRemovedEntry(document.Row, document.Column, document.Value ?? 0),
        "Cleared" => new PossibleValuesClearedEntry(document.Row, document.Column, document.PreviousValues),
        _ => throw new InvalidOperationException($"Unknown history entry type: {document.Type}")
    };

    private static CellDocument ToDocument(this Cell cell)
    {
        var cellDocument = new CellDocument
        {
            Row = cell.Row,
            Column = cell.Column,
            Value = cell.Value,
            IsFixed = cell.IsFixed,
            IsHint = cell.IsHint,
            PossibleValues = cell.PossibleValues.ToHashSet()
        };
        return cellDocument;
    }

    private static Cell ToDomain(this CellDocument document, BoardSize size)
    {
        var cell = Cell.Create(document.Row, document.Column, size, document.Value, document.IsFixed, document.IsHint);

        // Set possible values using reflection since there's no public setter
        var cellType = typeof(Cell);
        var possibleValuesField = cellType.GetField("<PossibleValues>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        possibleValuesField?.SetValue(cell, document.PossibleValues);

        return cell;
    }

    private static GameStatisticsDocument ToDocument(this GameStatistics statistics)
    {
        var gameStatisticsDocument = new GameStatisticsDocument
        {
            TotalMoves = statistics.TotalMoves,
            ValidMoves = statistics.ValidMoves,
            InvalidMoves = statistics.InvalidMoves,
            HintsUsed = statistics.HintsUsed,
            PlayDuration = statistics.PlayDuration,
            LastMoveAt = statistics.LastMoveAt
        };
        return gameStatisticsDocument;
    }

    private static GameStatistics ToDomain(this GameStatisticsDocument document)
    {
        var statistics = GameStatistics.Create();

        // Use reflection to restore the statistics state
        var statsType = typeof(GameStatistics);

        var totalMovesField = statsType.GetField("<TotalMoves>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        totalMovesField?.SetValue(statistics, document.TotalMoves);

        var validMovesField = statsType.GetField("<ValidMoves>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        validMovesField?.SetValue(statistics, document.ValidMoves);

        var invalidMovesField = statsType.GetField("<InvalidMoves>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        invalidMovesField?.SetValue(statistics, document.InvalidMoves);

        var hintsUsedField = statsType.GetField("<HintsUsed>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        hintsUsedField?.SetValue(statistics, document.HintsUsed);

        var playDurationField = statsType.GetField("<PlayDuration>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        playDurationField?.SetValue(statistics, document.PlayDuration);

        var lastMoveAtField = statsType.GetField("<LastMoveAt>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        lastMoveAtField?.SetValue(statistics, document.LastMoveAt);

        return statistics;
    }
}
