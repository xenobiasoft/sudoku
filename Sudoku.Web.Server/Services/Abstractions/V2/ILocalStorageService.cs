using Sudoku.Web.Server.Models;

namespace Sudoku.Web.Server.Services.Abstractions.V2;

public interface ILocalStorageService
{
    Task DeleteGameAsync(string gameId);
    Task<string?> GetAliasAsync();
    Task<GameModel?> LoadGameAsync(string gameId);
    Task<List<GameModel>> LoadGameStatesAsync();
    Task SaveGameStateAsync(GameModel gameState);
    Task SetAliasAsync(string alias);
}