using Microsoft.Extensions.Logging;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Queries;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class GetProfileByIdQueryHandler(
    IUserProfileRepository profileRepository,
    ILogger<GetProfileByIdQueryHandler> logger) : IQueryHandler<GetProfileByIdQuery, ProfileDto?>
{
    public async Task<Result<ProfileDto?>> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.ProfileId, out var guid))
        {
            logger.LogWarning("Invalid ProfileId format: {ProfileId}", request.ProfileId);
            return Result<ProfileDto?>.Failure($"Invalid ProfileId format: {request.ProfileId}");
        }

        try
        {
            var profileId = ProfileId.From(guid);
            var profile = await profileRepository.GetByIdAsync(profileId);

            if (profile == null)
            {
                logger.LogDebug("Profile not found for id {ProfileId}", request.ProfileId);
                return Result<ProfileDto?>.Success(null);
            }

            logger.LogDebug("Retrieved profile {ProfileId}", request.ProfileId);
            return Result<ProfileDto?>.Success(ProfileDto.FromProfile(profile));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error getting profile for id {ProfileId}", request.ProfileId);
            return Result<ProfileDto?>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}
