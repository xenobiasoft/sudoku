using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public interface IGameStorageManager
{
    Task DeleteGameAsync(string gameId);
    Task<GameStateMemory?> LoadGameAsync(string gameId);
    Task SaveGameAsync(GameStateMemory gameState);
    Task<GameStateMemory> UndoAsync(string gameId);
}