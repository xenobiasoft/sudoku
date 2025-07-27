using Microsoft.Extensions.Logging;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class DeletePlayerGamesCommandHandler(IGameRepository gameRepository, ILogger<DeletePlayerGamesCommandHandler> logger) : ICommandHandler<DeletePlayerGamesCommand>
{
    public async Task<Result> Handle(DeletePlayerGamesCommand cmd, CancellationToken cancellationToken)
    {
        try
        {
            var playerAlias = PlayerAlias.Create(cmd.PlayerAlias);
            
            var games = await gameRepository.GetByPlayerAsync(playerAlias);
            
            foreach (var game in games)
            {
                await gameRepository.DeleteAsync(game.Id);
            }
            
            logger.LogInformation("Deleted {Count} games for player {PlayerAlias}", games.Count(), playerAlias.Value);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Failed to delete player games: {Error}", ex.Message);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred deleting player games");
            return Result.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}