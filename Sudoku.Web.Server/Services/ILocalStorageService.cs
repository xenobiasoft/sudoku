using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

public interface ILocalStorageService
{
    Task DeleteGameStateAsync();
    Task<List<GameStateMemory>> LoadGameStatesAsync();
    Task SaveGameStateAsync(GameStateMemory gameState);
}