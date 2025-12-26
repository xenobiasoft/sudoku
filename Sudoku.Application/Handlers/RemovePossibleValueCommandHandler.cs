using Microsoft.Extensions.Logging;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class RemovePossibleValueCommandHandler(IGameRepository gameRepository, ILogger<RemovePossibleValueCommandHandler> logger) : ICommandHandler<RemovePossibleValueCommand>
{
    public async Task<Result> Handle(RemovePossibleValueCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var gameId = GameId.Create(request.GameId);
            var game = await gameRepository.GetByIdAsync(gameId);

            if (game == null)
            {
                return Result.Failure($"Game not found with ID: {request.GameId}");
            }

            game.RemovePossibleValue(request.Row, request.Column, request.Value);

            await gameRepository.SaveAsync(game);

            logger.LogInformation("Removed possible value {Value} from game {GameId} at [{Row},{Column}]",
                request.Value, gameId.Value, request.Row, request.Column);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Failed to remove possible value from game {GameId}: {Error}", request.GameId, ex.Message);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred removing possible value from game {GameId}", request.GameId);
            return Result.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}