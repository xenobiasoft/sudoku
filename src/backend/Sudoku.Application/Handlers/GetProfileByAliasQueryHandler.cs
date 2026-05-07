using Microsoft.Extensions.Logging;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Queries;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class GetProfileByAliasQueryHandler(
    IUserProfileRepository profileRepository,
    ILogger<GetProfileByAliasQueryHandler> logger) : IQueryHandler<GetProfileByAliasQuery, ProfileDto?>
{
    public async Task<Result<ProfileDto?>> Handle(GetProfileByAliasQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var playerAlias = PlayerAlias.Create(request.Alias.Trim());
            var profile = await profileRepository.GetByAliasAsync(playerAlias);

            if (profile == null)
            {
                logger.LogDebug("Profile not found for alias {Alias}", playerAlias.Value);
                return Result<ProfileDto?>.Success(null);
            }

            logger.LogDebug("Retrieved profile {ProfileId} for alias {Alias}", profile.Id, playerAlias.Value);
            return Result<ProfileDto?>.Success(ProfileDto.FromProfile(profile));
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Domain error getting profile for alias {Alias}: {Error}", request.Alias, ex.Message);
            return Result<ProfileDto?>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error getting profile for alias {Alias}", request.Alias);
            return Result<ProfileDto?>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}
