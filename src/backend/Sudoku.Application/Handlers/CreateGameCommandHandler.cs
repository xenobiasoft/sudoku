using Microsoft.Extensions.Logging;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class CreateGameCommandHandler(IGameRepository gameRepository, IPuzzleGenerator puzzleGenerator, ILogger<CreateGameCommandHandler> logger) : ICommandHandler<CreateGameCommand>
{
    public async Task<Result> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var playerAlias = PlayerAlias.Create(request.PlayerAlias);
            var difficulty = GameDifficulty.FromName(request.Difficulty);
            var puzzle = await puzzleGenerator.GeneratePuzzleAsync(difficulty);

            if (puzzle == null)
            {
                return Result.Failure($"No puzzle available for difficulty: {difficulty.Name}");
            }

            var game = SudokuGame.Create(playerAlias, difficulty, puzzle.Cells);

            await gameRepository.SaveAsync(game);

            logger.LogInformation("Created game {GameId} for player {PlayerAlias} with difficulty {Difficulty}", 
                game.Id.Value, playerAlias.Value, difficulty.Name);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Failed to create game for player {PlayerAlias}: {Error}", request.PlayerAlias, ex.Message);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred creating game for player {PlayerAlias}", request.PlayerAlias);
            return Result.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}