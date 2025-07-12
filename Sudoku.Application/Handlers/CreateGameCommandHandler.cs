using MediatR;
using Sudoku.Application.Common;
using Sudoku.Application.Commands;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class CreateGameCommandHandler : ICommandHandler<CreateGameCommand>
{
    private readonly IGameRepository _gameRepository;
    private readonly IPuzzleRepository _puzzleRepository;

    public CreateGameCommandHandler(IGameRepository gameRepository, IPuzzleRepository puzzleRepository)
    {
        _gameRepository = gameRepository;
        _puzzleRepository = puzzleRepository;
    }

    public async Task<Result> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate and create value objects
            var playerAlias = PlayerAlias.Create(request.PlayerAlias);
            var difficulty = GameDifficulty.FromName(request.Difficulty);

            // Get a random puzzle for the specified difficulty
            var puzzle = await _puzzleRepository.GetRandomByDifficultyAsync(difficulty);
            if (puzzle == null)
                return Result.Failure($"No puzzle available for difficulty: {difficulty.Name}");

            // Create the game with the puzzle cells
            var game = SudokuGame.Create(playerAlias, difficulty, puzzle.Cells);

            // Save the game
            await _gameRepository.SaveAsync(game);

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