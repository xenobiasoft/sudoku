using Sudoku.Domain.Entities;
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
            Status = game.Status,
            Cells = game.GetCells().Select(ToDocument).ToList(),
            Statistics = ToDocument(game.Statistics),
            MoveHistory = game.MoveHistory
                .Select(m => new MoveHistoryDocument
                {
                    Row = m.Row,
                    Column = m.Column,
                    PreviousValue = m.PreviousValue,
                    NewValue = m.NewValue
                })
                .ToList(),
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

        var sudokuGame = SudokuGame.Reconstitute(
            GameId.Create(document.GameId),
            profileId,
            displayName,
            difficulty,
            document.Status,
            document.Statistics.ToDomain(),
            document.Cells.Select(ToDomain).ToList(),
            document.MoveHistory.Select(m => new MoveHistory(m.Row, m.Column, m.PreviousValue, m.NewValue)),
            document.CreatedAt,
            document.StartedAt,
            document.CompletedAt,
            document.PausedAt
        );

        return sudokuGame;
    }

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

    private static Cell ToDomain(this CellDocument document)
    {
        var cell = Cell.Create(document.Row, document.Column, document.Value, document.IsFixed, document.IsHint);

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
