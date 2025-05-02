using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public interface ILocalStorageService
{
    Task AddGameStateAsync(GameStateMemory gameState);
    Task ClearGameStateAsync();
    Task<List<GameStateMemory>> LoadGameStatesAsync();
    Task RemoveGameAsync(string gameId);
}