using Microsoft.Extensions.Logging;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class MakeMoveCommandHandler(IGameRepository gameRepository, ILogger<MakeMoveCommandHandler> logger) : ICommandHandler<MakeMoveCommand>
{
    public async Task<Result> Handle(MakeMoveCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var gameId = GameId.Create(request.GameId);
            var game = await gameRepository.GetByIdAsync(gameId);

            if (game == null)
            {
                return Result.Failure($"Game not found with ID: {request.GameId}");
            }

            game.MakeMove(request.Row, request.Column, request.Value);
            game.UpdatePlayDuration(request.PlayDuration);

            await gameRepository.SaveAsync(game);

            logger.LogInformation("Made move for game {GameId} at [{Row},{Column}] with value {Value}", 
                gameId.Value, request.Row, request.Column, request.Value);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Failed to make move for game {GameId}: {Error}", request.GameId, ex.Message);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred making move for game {GameId}", request.GameId);
            return Result.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}