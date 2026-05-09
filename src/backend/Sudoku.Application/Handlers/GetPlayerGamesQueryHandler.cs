using Microsoft.Extensions.Logging;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Queries;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class GetPlayerGamesQueryHandler(IGameRepository gameRepository, ILogger<GetPlayerGamesQueryHandler> logger) : IQueryHandler<GetPlayerGamesQuery, List<GameDto>>
{
    public async Task<Result<List<GameDto>>> Handle(GetPlayerGamesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var profileId = ProfileId.From(request.ProfileId);
            var games = await gameRepository.GetByProfileIdAsync(profileId);
            var gameDtos = games.Select(GameDto.FromGame).ToList();

            logger.LogInformation("Retrieved {Count} games for profile {ProfileId}", gameDtos.Count, profileId.Value);
            return Result<List<GameDto>>.Success(gameDtos);
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Failed to get games for profile {ProfileId}: {Error}", request.ProfileId, ex.Message);
            return Result<List<GameDto>>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred getting games for profile {ProfileId}", request.ProfileId);
            return Result<List<GameDto>>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}
