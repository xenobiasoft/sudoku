using Microsoft.Extensions.Logging;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class CreateGameCommandHandler(IGameRepository gameRepository, IPuzzleGenerator puzzleGenerator, ILogger<CreateGameCommandHandler> logger) : ICommandHandler<CreateGameCommand, string>
{
    public async Task<Result<string>> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var profileId = ProfileId.From(request.ProfileId);
            var displayName = PlayerAlias.Create(request.DisplayName);
            var difficulty = GameDifficulty.FromName(request.Difficulty);
            var puzzle = await puzzleGenerator.GeneratePuzzleAsync(difficulty);

            if (puzzle == null)
            {
                return Result<string>.Failure($"No puzzle available for difficulty: {difficulty.Name}");
            }

            var game = SudokuGame.Create(profileId, displayName, difficulty, puzzle.Cells);

            await gameRepository.SaveAsync(game);

            logger.LogInformation("Created game {GameId} for profile {ProfileId} with difficulty {Difficulty}",
                game.Id.Value, profileId.Value, difficulty.Name);
            return Result<string>.Success(game.Id.Value.ToString());
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Failed to create game for profile {ProfileId}: {Error}", request.ProfileId, ex.Message);
            return Result<string>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred creating game for profile {ProfileId}", request.ProfileId);
            return Result<string>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}
