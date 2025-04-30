using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku;

public interface ISudokuGame
{
	Task<GameStateMemento> LoadAsync(string puzzleId);
	Task<string> NewGameAsync(Level level);
}