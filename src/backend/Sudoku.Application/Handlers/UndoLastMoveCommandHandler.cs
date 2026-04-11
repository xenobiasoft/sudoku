using Microsoft.Extensions.Logging;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class UndoLastMoveCommandHandler(IGameRepository gameRepository, ILogger<UndoLastMoveCommandHandler> logger) : ICommandHandler<UndoLastMoveCommand>
{
    public async Task<Result> Handle(UndoLastMoveCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var gameId = GameId.Create(request.GameId);
            
            var game = await gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                return Result.Failure($"Game not found with ID: {request.GameId}");
            }
            
            game.UndoLastMove();
            
            await gameRepository.SaveAsync(game);
            
            logger.LogInformation("Undid last move for game with ID: {GameId}", gameId.Value);
            return Result.Success();
        }
        catch (NoMoveHistoryException ex)
        {
            logger.LogWarning("No moves to undo for game with ID: {GameId}. Error: {Error}", request.GameId, ex.Message);
            return Result.Failure(ex.Message);
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Failed to undo last move: {Error}", ex.Message);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred undoing last move");
            return Result.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}