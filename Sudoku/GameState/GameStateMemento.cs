namespace XenobiaSoft.Sudoku.GameState;

public class GameStateMemento(ISudokuPuzzle puzzle, int score)
{
    public ISudokuPuzzle Puzzle { get; private set; } = puzzle;
    public int Score { get; private set; } = score;
}