using Sudoku.Web.Server.Models;

namespace Sudoku.Web.Server.Services.Abstractions;

/// <summary>
/// Interface for API-based game state management
/// </summary>
public interface IGameStateManager : IGameProvider
{
    Task AddPossibleValueAsync(int row, int column, int value);
    Task ClearPossibleValuesAsync(int row, int column);
    Task<GameModel> CreateGameAsync(string alias, string difficulty);
    Task DeleteGameAsync();
    Task DeleteGameAsync(string alias, string gameId);
    Task<GameModel> LoadGameAsync(string alias, string gameId);
    Task<List<GameModel>> LoadGamesAsync(string alias);
    Task RemovePossibleValueAsync(int row, int column, int value);
    Task<GameModel> ResetGameAsync();
    Task SaveGameAsync();
    Task SaveGameAsync(int row, int column, int? value);
    Task SaveGameStatusAsync();
    Task<GameModel> UndoGameAsync();
}