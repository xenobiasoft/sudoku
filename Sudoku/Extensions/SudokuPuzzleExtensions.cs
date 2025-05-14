using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku.Extensions;

public static class SudokuPuzzleExtensions
{
    public static PuzzleState ToPuzzleState(this ISudokuPuzzle puzzle)
    {
        return new PuzzleState(puzzle.PuzzleId, puzzle.GetAllCells());
    }
}