namespace Sudoku.Web.Server.Models;

/// <summary>
/// Represents a game in the Blazor application
/// </summary>
public class GameModel
{
    public string Id { get; set; } = string.Empty;
    public string PlayerAlias { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public GameStatisticsModel Statistics { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? PausedAt { get; set; }
    public List<CellModel> Cells { get; set; } = new();
}