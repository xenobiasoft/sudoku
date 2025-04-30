using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku;

public interface ISudokuGame
{
    Task DeleteAsync(string puzzleId);
    Task<GameStateMemento> LoadAsync(string puzzleId);
    Task<string> NewGameAsync(Level level);
    Task SaveAsync(GameStateMemento memento);
}