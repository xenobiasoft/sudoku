using Microsoft.Extensions.Logging;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class PauseGameCommandHandler(IGameRepository gameRepository, ILogger<PauseGameCommandHandler> logger) : ICommandHandler<PauseGameCommand>
{
    public async Task<Result> Handle(PauseGameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var gameId = GameId.Create(request.GameId);
            var game = await gameRepository.GetByIdAsync(gameId);

            if (game == null)
            {
                return Result.Failure($"Game not found with ID: {request.GameId}");
            }

            game.PauseGame();

            await gameRepository.SaveAsync(game);

            logger.LogInformation("Paused game with ID: {GameId}", gameId.Value);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Failed to pause game {GameId}: {Error}", request.GameId, ex.Message);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred pausing game {GameId}", request.GameId);
            return Result.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}