namespace Sudoku.Web.Server.Models;

/// <summary>
/// Represents a move in the game history
/// </summary>
public class MoveHistoryModel
{
    public int Row { get; set; }
    public int Column { get; set; }
    public int? PreviousValue { get; set; }
    public int? NewValue { get; set; }
}
