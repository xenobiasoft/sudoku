using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku;

public interface ISudokuGame
{
    Task DeleteAsync(string alias, string puzzleId);
    Task<GameStateMemory> LoadAsync(string alias, string puzzleId);
    Task<GameStateMemory> NewGameAsync(string alias, GameDifficulty difficulty);
    Task SaveAsync(GameStateMemory memory);
}