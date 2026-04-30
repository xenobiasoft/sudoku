using Microsoft.Extensions.Logging;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class CreateProfileCommandHandler(
    IUserProfileRepository profileRepository,
    ILogger<CreateProfileCommandHandler> logger) : ICommandHandler<CreateProfileCommand, ProfileDto>
{
    public async Task<Result<ProfileDto>> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var normalizedAlias = request.Alias.Trim().ToLowerInvariant();
            var playerAlias = PlayerAlias.Create(normalizedAlias);

            if (await profileRepository.AliasExistsAsync(playerAlias))
            {
                logger.LogWarning("Alias already taken: {Alias}", playerAlias.Value);
                return Result<ProfileDto>.Failure($"Alias '{playerAlias.Value}' is already taken.");
            }

            var profile = Domain.Entities.UserProfile.Create(playerAlias);
            await profileRepository.SaveAsync(profile);

            logger.LogInformation("Created profile {ProfileId} for alias {Alias}", profile.Id, playerAlias.Value);
            return Result<ProfileDto>.Success(ProfileDto.FromProfile(profile));
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Domain error creating profile: {Error}", ex.Message);
            return Result<ProfileDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating profile for alias {Alias}", request.Alias);
            return Result<ProfileDto>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}
