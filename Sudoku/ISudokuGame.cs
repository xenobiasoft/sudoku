using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku;

public interface ISudokuGame
{
    Task DeleteAsync(string puzzleId);
    Task<GameStateMemory> LoadAsync(string puzzleId);
    Task<GameStateMemory> NewGameAsync(Level level);
    Task SaveAsync(GameStateMemory memory);
}