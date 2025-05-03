using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public interface ILocalStorageService
{
    Task SaveGameStateAsync(GameStateMemory gameState);
    Task<GameStateMemory?> LoadGameAsync(string gameId);
    Task<List<GameStateMemory>> LoadGameStatesAsync();
    Task DeleteGameAsync(string gameId);
}