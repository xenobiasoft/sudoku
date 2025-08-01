using MediatR;
using Sudoku.Application.Common;
using Sudoku.Application.Commands;
using Sudoku.Application.Queries;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;

namespace Sudoku.Application.Services;

public class GameApplicationService(IMediator mediator) : IGameApplicationService
{
    public async Task<Result<GameDto>> CreateGameAsync(string playerAlias, string difficulty)
    {
        var command = new CreateGameCommand(playerAlias, difficulty);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return Result<GameDto>.Failure(result.Error!);
        }

        // After creating the game, we need to get it back to return the DTO
        // This is a limitation of the current design - we could improve this by
        // having the command return the created game ID and then query for it
        var games = await GetPlayerGamesAsync(playerAlias);
        if (!games.IsSuccess)
        {
            return Result<GameDto>.Failure(games.Error!);
        }

        var latestGame = games.Value!.OrderByDescending(g => g.CreatedAt).FirstOrDefault();
        if (latestGame == null)
        {
            return Result<GameDto>.Failure("Failed to retrieve created game");
        }

        return Result<GameDto>.Success(latestGame);
    }

    public async Task<Result<GameDto>> GetGameAsync(string gameId)
    {
        var query = new GetGameQuery(gameId);
        return await mediator.Send(query);
    }

    public async Task<Result<List<GameDto>>> GetPlayerGamesAsync(string playerAlias)
    {
        var query = new GetPlayerGamesQuery(playerAlias);
        return await mediator.Send(query);
    }

    public async Task<Result<List<GameDto>>> GetPlayerGamesByStatusAsync(string playerAlias, string status)
    {
        var query = new GetPlayerGamesByStatusQuery(playerAlias, status);
        return await mediator.Send(query);
    }

    public async Task<Result> MakeMoveAsync(string gameId, int row, int column, int? value)
    {
        var command = new MakeMoveCommand(gameId, row, column, value);
        return await mediator.Send(command);
    }

    public async Task<Result> AddPossibleValueAsync(string gameId, int row, int column, int value)
    {
        var command = new AddPossibleValueCommand(gameId, row, column, value);
        return await mediator.Send(command);
    }

    public async Task<Result> RemovePossibleValueAsync(string gameId, int row, int column, int value)
    {
        var command = new RemovePossibleValueCommand(gameId, row, column, value);
        return await mediator.Send(command);
    }

    public async Task<Result> ClearPossibleValuesAsync(string gameId, int row, int column)
    {
        var command = new ClearPossibleValuesCommand(gameId, row, column);
        return await mediator.Send(command);
    }

    public async Task<Result> StartGameAsync(string gameId)
    {
        var command = new StartGameCommand(gameId);
        return await mediator.Send(command);
    }

    public async Task<Result> PauseGameAsync(string gameId)
    {
        var command = new PauseGameCommand(gameId);
        return await mediator.Send(command);
    }

    public async Task<Result> ResumeGameAsync(string gameId)
    {
        var command = new ResumeGameCommand(gameId);
        return await mediator.Send(command);
    }

    public async Task<Result> AbandonGameAsync(string gameId)
    {
        var command = new AbandonGameCommand(gameId);
        return await mediator.Send(command);
    }

    public async Task<Result> DeleteGameAsync(string gameId)
    {
        var command = new DeleteGameCommand(gameId);
        return await mediator.Send(command);
    }

    public async Task<Result> DeletePlayerGamesAsync(string playerAlias)
    {
        var command = new DeletePlayerGamesCommand(playerAlias);
        return await mediator.Send(command);
    }

    public async Task<Result> UndoLastMoveAsync(string gameId)
    {
        var command = new UndoLastMoveCommand(gameId);
        return await mediator.Send(command);
    }

    public async Task<Result> ResetGameAsync(string gameId)
    {
        var command = new ResetGameCommand(gameId);
        return await mediator.Send(command);
    }

    public async Task<Result<ValidationResultDto>> ValidateGameAsync(string gameId)
    {
        var query = new ValidateGameQuery(gameId);
        return await mediator.Send(query);
    }
}