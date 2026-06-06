namespace Sudoku.Infrastructure.Models;

public class SudokuPuzzleDocument
{
    public string PuzzleId { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public List<CellDocument> Cells { get; set; } = [];
}
