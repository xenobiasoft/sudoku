using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Queries;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class GetGameQueryHandler(IGameRepository gameRepository) : IQueryHandler<GetGameQuery, GameDto>
{
    public async Task<Result<GameDto>> Handle(GetGameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var gameId = GameId.Create(request.GameId);
            var game = await gameRepository.GetByIdAsync(gameId);

            if (game == null)
            {
                return Result<GameDto>.Failure($"Game not found with ID: {request.GameId}");
            }

            var gameDto = GameDto.FromGame(game);

            return Result<GameDto>.Success(gameDto);
        }
        catch (DomainException ex)
        {
            return Result<GameDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<GameDto>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}