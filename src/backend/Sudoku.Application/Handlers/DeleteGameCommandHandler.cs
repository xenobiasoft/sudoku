using Microsoft.Extensions.Logging;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Models;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class DeleteGameCommandHandler(
    IGameRepository gameRepository,
    IGameCompletionRepository completionRepository,
    ILogger<DeleteGameCommandHandler> logger) : ICommandHandler<DeleteGameCommand>
{
    public async Task<Result> Handle(DeleteGameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var gameId = GameId.Create(request.GameId);

            var game = await gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                return Result.Failure($"Game not found with ID: {request.GameId}");
            }

            await EnsureCompletionRecordedAsync(game);

            await gameRepository.DeleteAsync(gameId);

            logger.LogInformation("Deleted game with ID: {GameId}", gameId.Value);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Failed to delete game: {Error}", ex.Message);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred deleting game");
            return Result.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Backstop for the completion record. The client deletes a game moments after solving it,
    /// so a completed game must never be deleted without its win being recorded first — this
    /// closes the gap if <c>GameCompletedEventHandler</c> was skipped or failed. If the write
    /// throws, the delete is abandoned and the game document survives to be retried.
    /// </summary>
    private async Task EnsureCompletionRecordedAsync(SudokuGame game)
    {
        if (game.Status != GameStatusEnum.Completed)
        {
            return;
        }

        var existing = await completionRepository.GetByGameIdAsync(game.Id, game.ProfileId);
        if (existing != null)
        {
            return;
        }

        var completion = new GameCompletion(
            game.Id.ToString(),
            game.ProfileId.ToString(),
            game.Difficulty.Name,
            game.Statistics.PlayDuration,
            game.CompletedAt ?? DateTime.UtcNow);

        await completionRepository.UpsertAsync(completion);

        logger.LogInformation(
            "Backstop recorded completion for game {GameId} (profile {ProfileId}, {Difficulty}) at delete time",
            completion.GameId, completion.ProfileId, completion.Difficulty);
    }
}
