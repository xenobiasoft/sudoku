namespace Sudoku.Infrastructure.Models;

public class SudokuPuzzleDocument
{
    public string PuzzleId { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public int GridSize { get; set; } = 9;
    public List<CellDocument> Cells { get; set; } = [];
}
