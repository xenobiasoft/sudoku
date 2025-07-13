using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class CreateGameCommandHandler(IGameRepository gameRepository, IPuzzleRepository puzzleRepository) : ICommandHandler<CreateGameCommand>
{
    public async Task<Result> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var playerAlias = PlayerAlias.Create(request.PlayerAlias);
            var difficulty = GameDifficulty.FromName(request.Difficulty);
            var puzzle = await puzzleRepository.GetRandomByDifficultyAsync(difficulty);

            if (puzzle == null)
            {
                return Result.Failure($"No puzzle available for difficulty: {difficulty.Name}");
            }

            var game = SudokuGame.Create(playerAlias, difficulty, puzzle.Cells);

            await gameRepository.SaveAsync(game);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}