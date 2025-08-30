using Sudoku.Web.Server.Models;

namespace Sudoku.Web.Server.Services.Abstractions.V2;

/// <summary>
/// Interface for API-based game state management
/// </summary>
public interface IGameStateManager
{
    Task DeleteGameAsync(string alias, string gameId);
    Task<GameModel> LoadGameAsync(string alias, string gameId);
    Task<List<GameModel>> LoadGamesAsync();
    Task<GameModel> ResetGameAsync(string alias, string gameId);
    Task SaveGameAsync(GameModel gameState);
    Task<GameModel> UndoGameAsync(string alias, string gameId);
}