namespace Sudoku.Web.Server.Models;

public class SavedGame
{
    public string PuzzleId { get; set; } = string.Empty;
    public DateTime LastSaved { get; set; }
}