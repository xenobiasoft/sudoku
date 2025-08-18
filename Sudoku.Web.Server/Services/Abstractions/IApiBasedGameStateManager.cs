using Sudoku.Web.Server.Models;

namespace Sudoku.Web.Server.Services.Abstractions;

/// <summary>
/// Interface for API-based game state management
/// </summary>
public interface IApiBasedGameStateManager
{
    Task<ApiResult> DeleteGameAsync(string alias, string gameId);
    Task<ApiResult<GameModel>> LoadGameAsync(string alias, string gameId);
    Task<ApiResult<List<GameModel>>> LoadGamesAsync(string alias);
    Task<ApiResult> ResetGameAsync(string alias, string gameId);
    Task<ApiResult<GameModel>> CreateGameAsync(string alias, string difficulty);
    Task<ApiResult> UndoGameAsync(string alias, string gameId);
    Task<ApiResult> MakeMoveAsync(string alias, string gameId, int row, int column, int? value);
    Task<ApiResult> AddPossibleValueAsync(string alias, string gameId, int row, int column, int value);
    Task<ApiResult> RemovePossibleValueAsync(string alias, string gameId, int row, int column, int value);
    Task<ApiResult> ClearPossibleValuesAsync(string alias, string gameId, int row, int column);
    Task<ApiResult<ValidationResultModel>> ValidateGameAsync(string alias, string gameId);
}