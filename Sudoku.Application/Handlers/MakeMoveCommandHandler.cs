using MediatR;
using Sudoku.Application.Common;
using Sudoku.Application.Commands;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class MakeMoveCommandHandler : ICommandHandler<MakeMoveCommand>
{
    private readonly IGameRepository _gameRepository;

    public MakeMoveCommandHandler(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<Result> Handle(MakeMoveCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Parse the game ID
            var gameId = GameId.Create(request.GameId);

            // Get the game
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
                return Result.Failure($"Game not found with ID: {request.GameId}");

            // Make the move
            game.MakeMove(request.Row, request.Column, request.Value);

            // Save the updated game
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