using Sudoku.Web.Server.Models;

namespace Sudoku.Web.Server.Services.Abstractions.V2;

/// <summary>
/// Interface for API-based game state management
/// </summary>
public interface IGameStateManager
{
    GameModel Game { get; }
    Task<GameModel> CreateGameAsync(string alias, string difficulty);
    Task DeleteGameAsync();
    Task DeleteGameAsync(string alias, string gameId);
    Task<GameModel> LoadGameAsync(string alias, string gameId);
    Task<List<GameModel>> LoadGamesAsync(string alias);
    Task<GameModel> ResetGameAsync();
    Task SaveGameAsync();
    Task<GameModel> UndoGameAsync();
}