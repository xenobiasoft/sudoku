using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public interface IGameStateManager
{
    Task DeleteGameAsync(string gameId);
    Task<GameStateMemory?> LoadGameAsync(string gameId);
    Task<List<GameStateMemory>> LoadGamesAsync();
    Task<GameStateMemory> ResetGameAsync(string gameId);
    Task SaveGameAsync(GameStateMemory gameState);
    Task<GameStateMemory> UndoGameAsync(string gameId);
}