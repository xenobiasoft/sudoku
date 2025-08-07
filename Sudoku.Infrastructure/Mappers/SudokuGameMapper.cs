using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Models;

namespace Sudoku.Infrastructure.Mappers;

public static class SudokuGameMapper
{
    public static SudokuGameDocument ToDocument(SudokuGame game)
    {
        return new SudokuGameDocument
        {
            Id = game.Id.Value.ToString(),
            GameId = game.Id.Value.ToString(),
            PlayerAlias = game.PlayerAlias.Value,
            Difficulty = game.Difficulty,
            Status = game.Status,
            Cells = game.GetCells().Select(ToDocument).ToList(),
            Statistics = ToDocument(game.Statistics),
            MoveHistory = [], // Move history is private, will need domain changes or reflection if needed
            CreatedAt = game.CreatedAt,
            StartedAt = game.StartedAt,
            CompletedAt = game.CompletedAt,
            PausedAt = game.PausedAt
        };
    }

    public static SudokuGame FromDocument(SudokuGameDocument document)
    {
        var cells = document.Cells.Select(FromDocument).ToList();
        var playerAlias = PlayerAlias.Create(document.PlayerAlias);
        
        // Create game using factory method
        var game = SudokuGame.Create(playerAlias, document.Difficulty, cells);
        
        // Use reflection to set private fields since we need to restore the full state
        var gameType = typeof(SudokuGame);
        
        // Set the ID (override the generated one)
        var idField = gameType.GetField("<Id>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        idField?.SetValue(game, GameId.Create(document.GameId));
        
        // Set status
        var statusField = gameType.GetField("<Status>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        statusField?.SetValue(game, document.Status);
        
        // Set statistics
        var statisticsField = gameType.GetField("<Statistics>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        statisticsField?.SetValue(game, FromDocument(document.Statistics));
        
        // Set dates
        var createdAtField = gameType.GetField("<CreatedAt>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        createdAtField?.SetValue(game, document.CreatedAt);
        
        var startedAtField = gameType.GetField("<StartedAt>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        startedAtField?.SetValue(game, document.StartedAt);
        
        var completedAtField = gameType.GetField("<CompletedAt>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        completedAtField?.SetValue(game, document.CompletedAt);
        
        var pausedAtField = gameType.GetField("<PausedAt>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        pausedAtField?.SetValue(game, document.PausedAt);
        
        // Clear any domain events created during construction
        game.ClearDomainEvents();
        
        return game;
    }

    private static CellDocument ToDocument(Cell cell)
    {
        return new CellDocument
        {
            Row = cell.Row,
            Column = cell.Column,
            Value = cell.Value,
            IsFixed = cell.IsFixed,
            PossibleValues = cell.PossibleValues.ToHashSet()
        };
    }

    private static Cell FromDocument(CellDocument document)
    {
        var cell = Cell.Create(document.Row, document.Column, document.Value, document.IsFixed);
        
        // Set possible values using reflection since there's no public setter
        var cellType = typeof(Cell);
        var possibleValuesField = cellType.GetField("<PossibleValues>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        possibleValuesField?.SetValue(cell, document.PossibleValues);
        
        return cell;
    }

    private static GameStatisticsDocument ToDocument(GameStatistics statistics)
    {
        return new GameStatisticsDocument
        {
            TotalMoves = statistics.TotalMoves,
            ValidMoves = statistics.ValidMoves,
            InvalidMoves = statistics.InvalidMoves,
            PlayDuration = statistics.PlayDuration,
            LastMoveAt = statistics.LastMoveAt
        };
    }

    private static GameStatistics FromDocument(GameStatisticsDocument document)
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
        
        var playDurationField = statsType.GetField("<PlayDuration>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        playDurationField?.SetValue(statistics, document.PlayDuration);
        
        var lastMoveAtField = statsType.GetField("<LastMoveAt>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        lastMoveAtField?.SetValue(statistics, document.LastMoveAt);
        
        return statistics;
    }
}