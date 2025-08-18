using Sudoku.Web.Server.Models;

namespace Sudoku.Web.Server.Services.Abstractions.V2;

/// <summary>
/// Interface for API-based game state management
/// </summary>
public interface IGameStateManager
{
    Task<ApiResult<bool>> AddPossibleValueAsync(string alias, string gameId, int row, int column, int value);
    Task<ApiResult<bool>> ClearPossibleValuesAsync(string alias, string gameId, int row, int column);
    Task<ApiResult<GameModel>> CreateGameAsync(string alias, string difficulty);
    Task<ApiResult<bool>> DeleteGameAsync(string alias, string gameId);
    Task<ApiResult<GameModel>> LoadGameAsync(string alias, string gameId);
    Task<ApiResult<List<GameModel>>> LoadGamesAsync(string alias);
    Task<ApiResult<bool>> MakeMoveAsync(string alias, string gameId, int row, int column, int? value);
    Task<ApiResult<bool>> RemovePossibleValueAsync(string alias, string gameId, int row, int column, int value);
    Task<ApiResult<bool>> ResetGameAsync(string alias, string gameId);
    Task<ApiResult<bool>> UndoGameAsync(string alias, string gameId);
    Task<ApiResult<ValidationResultModel>> ValidateGameAsync(string alias, string gameId);
}