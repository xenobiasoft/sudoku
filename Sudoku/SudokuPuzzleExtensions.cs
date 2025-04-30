using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku;

public static class SudokuPuzzleExtensions
{
    public static GameStateMemento ToGameState(this ISudokuPuzzle puzzle, int score)
    {
        return new GameStateMemento(puzzle.PuzzleId, puzzle.GetAllCells(), score);
    }
}