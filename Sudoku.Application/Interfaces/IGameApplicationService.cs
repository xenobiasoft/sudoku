using Sudoku.Application.Common;
using Sudoku.Application.DTOs;

namespace Sudoku.Application.Interfaces;

public interface IGameApplicationService
{
    Task<Result<GameDto>> CreateGameAsync(string playerAlias, string difficulty);
    Task<Result<GameDto>> GetGameAsync(string gameId);
    Task<Result<List<GameDto>>> GetPlayerGamesAsync(string playerAlias);
    Task<Result<List<GameDto>>> GetPlayerGamesByStatusAsync(string playerAlias, string status);
    Task<Result> MakeMoveAsync(string gameId, int row, int column, int? value);
    Task<Result> StartGameAsync(string gameId);
    Task<Result> PauseGameAsync(string gameId);
    Task<Result> ResumeGameAsync(string gameId);
    Task<Result> AbandonGameAsync(string gameId);
    
    // New methods to support the API endpoints
    Task<Result> DeleteGameAsync(string gameId);
    Task<Result> DeletePlayerGamesAsync(string playerAlias);
    Task<Result> UndoLastMoveAsync(string gameId);
    Task<Result> ResetGameAsync(string gameId);
    Task<Result<ValidationResultDto>> ValidateGameAsync(string gameId);
}