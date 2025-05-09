using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku;

public static class SudokuPuzzleExtensions
{
    public static PuzzleState ToPuzzleState(this ISudokuPuzzle puzzle)
    {
        return new PuzzleState(puzzle.PuzzleId, puzzle.GetAllCells());
    }
}