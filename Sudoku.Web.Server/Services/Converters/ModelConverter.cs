using Sudoku.Web.Server.Models;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services.Converters;

/// <summary>
/// Converts between old legacy models and new API models
/// </summary>
public static class ModelConverter
{
    /// <summary>
    /// Converts a GameModel from the API to a GameStateMemory for legacy components
    /// </summary>
    public static GameStateMemory ToGameStateMemory(GameModel gameModel)
    {
        return new GameStateMemory
        {
            Alias = gameModel.PlayerAlias,
            PuzzleId = gameModel.Id,
            TotalMoves = gameModel.Statistics.TotalMoves,
            InvalidMoves = gameModel.Statistics.InvalidMoves,
            PlayDuration = gameModel.Statistics.PlayDuration,
            LastUpdated = gameModel.StartedAt ?? gameModel.CreatedAt,
            Board = gameModel.Cells.Select(ToLegacyCell).ToArray()
        };
    }

    /// <summary>
    /// Converts a CellModel from the API to a legacy Cell
    /// </summary>
    public static Cell ToLegacyCell(CellModel cellModel)
    {
        return new Cell(cellModel.Row, cellModel.Column)
        {
            Value = cellModel.Value,
            Locked = cellModel.IsFixed,
            PossibleValues = cellModel.PossibleValues.ToList()
        };
    }

    /// <summary>
    /// Converts a legacy Cell to a CellModel for the API
    /// </summary>
    public static CellModel ToCellModel(Cell cell)
    {
        return new CellModel(
            cell.Row,
            cell.Column,
            cell.Value,
            cell.Locked,
            cell.Value.HasValue,
            cell.PossibleValues
        );
    }

    /// <summary>
    /// Converts a list of GameModels to a list of GameStateMemory objects
    /// </summary>
    public static List<GameStateMemory> ToGameStateMemoryList(List<GameModel> games)
    {
        return games.Select(ToGameStateMemory).ToList();
    }

    /// <summary>
    /// Gets the difficulty string from a GameDifficulty enum
    /// </summary>
    public static string GetDifficultyString(GameDifficulty difficulty)
    {
        return difficulty switch
        {
            GameDifficulty.Easy => "Easy",
            GameDifficulty.Medium => "Medium", 
            GameDifficulty.Hard => "Hard",
            GameDifficulty.ExtremelyHard => "Expert",
            _ => "Easy"
        };
    }

    /// <summary>
    /// Parses a difficulty string to a GameDifficulty enum
    /// </summary>
    public static GameDifficulty ParseDifficulty(string difficulty)
    {
        return difficulty?.ToLowerInvariant() switch
        {
            "easy" => GameDifficulty.Easy,
            "medium" => GameDifficulty.Medium,
            "hard" => GameDifficulty.Hard,
            "expert" => GameDifficulty.ExtremelyHard,
            _ => GameDifficulty.Easy
        };
    }
}