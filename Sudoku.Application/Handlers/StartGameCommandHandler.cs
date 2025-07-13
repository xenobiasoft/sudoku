using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class StartGameCommandHandler(IGameRepository gameRepository) : ICommandHandler<StartGameCommand>
{
    public async Task<Result> Handle(StartGameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var gameId = GameId.Create(request.GameId);
            var game = await gameRepository.GetByIdAsync(gameId);

            if (game == null)
            {
                return Result.Failure($"Game not found with ID: {request.GameId}");
            }

            game.StartGame();

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