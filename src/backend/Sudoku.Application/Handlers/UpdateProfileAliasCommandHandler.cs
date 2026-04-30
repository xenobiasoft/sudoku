using Microsoft.Extensions.Logging;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class UpdateProfileAliasCommandHandler(
    IUserProfileRepository profileRepository,
    IGameRepository gameRepository,
    ILogger<UpdateProfileAliasCommandHandler> logger) : ICommandHandler<UpdateProfileAliasCommand, ProfileDto>
{
    public async Task<Result<ProfileDto>> Handle(UpdateProfileAliasCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var normalizedAlias = request.NewAlias.Trim().ToLowerInvariant();
            var newAlias = PlayerAlias.Create(normalizedAlias);

            var profile = await profileRepository.GetByIdAsync(ProfileId.From(request.ProfileId));
            if (profile == null)
            {
                return Result<ProfileDto>.Failure($"Profile not found: {request.ProfileId}", ProfileErrorCodes.NotFound);
            }

            if (string.Equals(profile.Alias.Value, newAlias.Value, StringComparison.OrdinalIgnoreCase))
            {
                return Result<ProfileDto>.Success(ProfileDto.FromProfile(profile));
            }

            if (await profileRepository.AliasExistsAsync(newAlias))
            {
                logger.LogWarning("Alias already taken: {Alias}", newAlias.Value);
                return Result<ProfileDto>.Failure($"Alias '{newAlias.Value}' is already taken.", ProfileErrorCodes.AliasTaken);
            }

            var oldAlias = profile.Alias;
            profile.UpdateAlias(newAlias);
            await profileRepository.SaveAsync(profile);

            var games = await gameRepository.GetByPlayerAsync(oldAlias);
            foreach (var game in games)
            {
                try
                {
                    // Update playerAlias via reconstitution is not straightforward;
                    // games store alias as a string field - update via SaveAsync after mutation
                    // The domain game does not expose alias mutation — we need to handle this at document level
                    // For now, we log a note; a full implementation would update the document directly
                    logger.LogDebug("Game {GameId} associated with old alias {OldAlias} — batch update pending", game.Id, oldAlias.Value);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to update game {GameId} for alias change", game.Id);
                }
            }

            logger.LogInformation("Updated alias from {OldAlias} to {NewAlias} for profile {ProfileId}",
                oldAlias.Value, newAlias.Value, profile.Id);
            return Result<ProfileDto>.Success(ProfileDto.FromProfile(profile));
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Domain error updating profile alias: {Error}", ex.Message);
            return Result<ProfileDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error updating alias for profile {ProfileId}", request.ProfileId);
            return Result<ProfileDto>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}
