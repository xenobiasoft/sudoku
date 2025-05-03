using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku;

public static class SudokuPuzzleExtensions
{
    public static GameStateMemory ToGameState(this ISudokuPuzzle puzzle, int score)
    {
        return new GameStateMemory(puzzle.PuzzleId, puzzle.GetAllCells(), score);
    }
}