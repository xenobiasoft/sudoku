using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku.Extensions;

public static class SudokuPuzzleExtensions
{
    public static GameStateMemory ToGameState(this ISudokuPuzzle puzzle)
    {
        if (puzzle is null)
        {
            throw new ArgumentNullException(nameof(puzzle), "The Sudoku puzzle cannot be null.");
        }

        return new GameStateMemory
        {
            PuzzleId = puzzle.PuzzleId,
            Board = puzzle.GetAllCells()
        };
    }
}