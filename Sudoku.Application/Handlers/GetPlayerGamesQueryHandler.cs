using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Queries;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class GetPlayerGamesQueryHandler(IGameRepository gameRepository) : IQueryHandler<GetPlayerGamesQuery, List<GameDto>>
{
    public async Task<Result<List<GameDto>>> Handle(GetPlayerGamesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var playerAlias = PlayerAlias.Create(request.PlayerAlias);
            var games = await gameRepository.GetByPlayerAsync(playerAlias);
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