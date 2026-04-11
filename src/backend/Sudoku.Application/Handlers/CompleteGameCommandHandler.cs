using Microsoft.Extensions.Logging;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class CompleteGameCommandHandler(IGameRepository gameRepository, ILogger<CompleteGameCommandHandler> logger) : ICommandHandler<CompleteGameCommand>
{
    public async Task<Result> Handle(CompleteGameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var gameId = GameId.Create(request.GameId);
            var game = await gameRepository.GetByIdAsync(gameId);

            if (game == null)
            {
                return Result.Failure($"Game not found with ID: {request.GameId}");
            }

            game.CompleteGame();

            await gameRepository.SaveAsync(game);

            logger.LogInformation("Completed game with ID: {GameId}", gameId.Value);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Failed to complete game {GameId}: {Error}", request.GameId, ex.Message);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred completing game {GameId}", request.GameId);
            return Result.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}
