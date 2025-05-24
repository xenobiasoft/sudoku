using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services.Abstractions;

public interface ILocalStorageService
{
    Task DeleteGameAsync(string gameId);
    Task<string?> GetAliasAsync();
    Task<GameStateMemory?> LoadGameAsync(string gameId);
    Task<List<GameStateMemory>> LoadGameStatesAsync();
    Task SaveGameStateAsync(GameStateMemory gameState);
    Task SetAliasAsync(string alias);
}