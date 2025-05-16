using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public interface IGameStateManager
{
    Task DeleteGameAsync(string gameId);
    Task<GameStateMemory?> LoadGameAsync(string gameId);
    Task<List<GameStateMemory>> LoadGamesAsync();
    Task<GameStateMemory> ResetAsync(string gameId);
    Task SaveGameAsync(GameStateMemory gameState);
    Task<GameStateMemory> UndoAsync(string gameId);
}