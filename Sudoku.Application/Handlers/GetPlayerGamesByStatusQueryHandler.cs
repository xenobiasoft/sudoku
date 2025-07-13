using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Queries;
using Sudoku.Domain.Enums;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class GetPlayerGamesByStatusQueryHandler(IGameRepository gameRepository)
    : IQueryHandler<GetPlayerGamesByStatusQuery, List<GameDto>>
{
    public async Task<Result<List<GameDto>>> Handle(GetPlayerGamesByStatusQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var playerAlias = PlayerAlias.Create(request.PlayerAlias);

            if (!Enum.TryParse<GameStatus>(request.Status, true, out var gameStatus))
            {
                return Result<List<GameDto>>.Failure($"Invalid game status: {request.Status}");
            }

            var games = await gameRepository.GetByPlayerAndStatusAsync(playerAlias, gameStatus);

            var gameDtos = games.Select(GameDto.FromGame).ToList();

            return Result<List<GameDto>>.Success(gameDtos);
        }
        catch (DomainException ex)
        {
            return Result<List<GameDto>>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<List<GameDto>>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}