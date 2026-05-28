using MediatR;
using Microsoft.Extensions.Logging;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class DeleteProfileCommandHandler(
    IUserProfileRepository profileRepository,
    IMediator mediator,
    ILogger<DeleteProfileCommandHandler> logger) : ICommandHandler<DeleteProfileCommand>
{
    public async Task<Result> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var alias = PlayerAlias.Create(request.Alias.Trim());
            var profile = await profileRepository.GetByAliasAsync(alias);

            if (profile == null)
            {
                return Result.Failure($"Profile not found: {request.Alias}", ProfileErrorCodes.NotFound);
            }

            var deleteGamesResult = await mediator.Send(new DeletePlayerGamesCommand(profile.Id.ToString()), cancellationToken);
            if (!deleteGamesResult.IsSuccess)
            {
                logger.LogWarning("Failed to delete games for profile {ProfileId}: {Error}", profile.Id, deleteGamesResult.Error);
                return deleteGamesResult;
            }

            await profileRepository.DeleteAsync(profile.Id);

            logger.LogInformation("Deleted profile {ProfileId} with alias {Alias}", profile.Id, alias.Value);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Domain error deleting profile with alias {Alias}: {Error}", request.Alias, ex.Message);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error deleting profile with alias {Alias}", request.Alias);
            return Result.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}
