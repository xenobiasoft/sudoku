using Microsoft.Extensions.Logging;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class RequestHintCommandHandler(
    IGameRepository gameRepository,
    IPuzzleSolver puzzleSolver,
    ILogger<RequestHintCommandHandler> logger) : ICommandHandler<RequestHintCommand>
{
    public async Task<Result> Handle(RequestHintCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var gameId = GameId.Create(request.GameId);
            var game = await gameRepository.GetByIdAsync(gameId);

            if (game == null)
            {
                return Result.Failure($"Game not found with ID: {request.GameId}");
            }

            // Solve from the fixed clues (and any previously revealed hints) only, so a wrong
            // player entry can never make the board unsolvable or yield the wrong hint value.
            var solvedPuzzle = await puzzleSolver.SolvePuzzle(BuildPuzzleFromClues(game));

            var (row, column, value) = game.RevealHint(solvedPuzzle);
            game.UpdatePlayDuration(request.PlayDuration);

            await gameRepository.SaveAsync(game);

            logger.LogDebug("Revealed hint for game {GameId} at [{Row},{Column}] with value {Value}",
                gameId.Value, row, column, value);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Failed to reveal hint for game {GameId}: {Error}", request.GameId, ex.Message);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred revealing hint for game {GameId}", request.GameId);
            return Result.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }

    private static SudokuPuzzle BuildPuzzleFromClues(SudokuGame game)
    {
        var cells = game.GetCells()
            .Select(c => c.IsLocked
                ? Cell.CreateFixed(c.Row, c.Column, c.Value!.Value)
                : Cell.CreateEmpty(c.Row, c.Column))
            .ToList();

        return SudokuPuzzle.Create(game.Id.Value.ToString(), game.Difficulty, cells);
    }
}
