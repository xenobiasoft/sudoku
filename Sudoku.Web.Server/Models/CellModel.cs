namespace Sudoku.Web.Server.Models;

/// <summary>
/// Represents a cell in the game
/// </summary>
public class CellModel
{
    public int Row { get; set; }
    public int Column { get; set; }
    public int? Value { get; set; }
    public bool IsFixed { get; set; }
    public bool HasValue { get; set; }
    public List<int> PossibleValues { get; set; } = new();
}