using Microsoft.Extensions.Logging;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Queries;
using Sudoku.Domain.Enums;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class GetPlayerGamesByStatusQueryHandler(IGameRepository gameRepository, ILogger<GetPlayerGamesByStatusQueryHandler> logger)
    : IQueryHandler<GetPlayerGamesByStatusQuery, List<GameDto>>
{
    public async Task<Result<List<GameDto>>> Handle(GetPlayerGamesByStatusQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var playerAlias = PlayerAlias.Create(request.PlayerAlias);

            if (!Enum.TryParse<GameStatusEnum>(request.Status, true, out var gameStatus))
            {
                return Result<List<GameDto>>.Failure($"Invalid game statusEnum: {request.Status}");
            }

            var games = await gameRepository.GetByPlayerAndStatusAsync(playerAlias, gameStatus);

            var gameDtos = games.Select(GameDto.FromGame).ToList();

            logger.LogInformation("Retrieved {Count} games with status {Status} for player {PlayerAlias}", 
                gameDtos.Count, gameStatus, playerAlias.Value);
            return Result<List<GameDto>>.Success(gameDtos);
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Failed to get games by status for player {PlayerAlias}: {Error}", request.PlayerAlias, ex.Message);
            return Result<List<GameDto>>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred getting games by status for player {PlayerAlias}", request.PlayerAlias);
            return Result<List<GameDto>>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}