using Sudoku.Blazor.Models;

namespace Sudoku.Blazor.Services.Abstractions;

public interface ILocalStorageService
{
    Task DeleteGameAsync(string gameId);
    Task<ProfileInfo?> GetProfileAsync();
    Task<GameModel?> LoadGameAsync(string gameId);
    Task<List<GameModel>> LoadGameStatesAsync();
    Task SaveGameStateAsync(GameModel gameState);
    Task SetProfileAsync(ProfileInfo profile);
}
