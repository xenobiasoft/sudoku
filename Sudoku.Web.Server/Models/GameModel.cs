namespace Sudoku.Web.Server.Models;

/// <summary>
/// Represents a game in the Blazor application
/// </summary>
public record GameModel(
    string Id,
    string PlayerAlias,
    string Difficulty,
    string Status,
    GameStatisticsModel Statistics,
    DateTime CreatedAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    DateTime? PausedAt,
    List<CellModel> Cells);