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
            var profileId = ProfileId.From(request.ProfileId);

            if (!Enum.TryParse<GameStatusEnum>(request.Status, true, out var gameStatus))
            {
                return Result<List<GameDto>>.Failure($"Invalid game statusEnum: {request.Status}");
            }

            var games = await gameRepository.GetByProfileIdAndStatusAsync(profileId, gameStatus);

            var gameDtos = games.Select(GameDto.FromGame).ToList();

            logger.LogDebug("Retrieved {Count} games with status {Status} for profile {ProfileId}",
                gameDtos.Count, gameStatus, profileId.Value);
            return Result<List<GameDto>>.Success(gameDtos);
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Failed to get games by status for profile {ProfileId}: {Error}", request.ProfileId, ex.Message);
            return Result<List<GameDto>>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred getting games by status for profile {ProfileId}", request.ProfileId);
            return Result<List<GameDto>>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}
