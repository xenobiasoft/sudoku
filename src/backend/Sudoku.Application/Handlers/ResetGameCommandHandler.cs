using Microsoft.Extensions.Logging;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class ResetGameCommandHandler(IGameRepository gameRepository, ILogger<ResetGameCommandHandler> logger) : ICommandHandler<ResetGameCommand>
{
    public async Task<Result> Handle(ResetGameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var gameId = GameId.Create(request.GameId);
            
            var game = await gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                return Result.Failure($"Game not found with ID: {request.GameId}");
            }
            
            game.ResetGame();
            
            await gameRepository.SaveAsync(game);
            
            logger.LogInformation("Reset game with ID: {GameId}", gameId.Value);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Failed to reset game: {Error}", ex.Message);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred resetting game");
            return Result.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}