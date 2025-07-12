using MediatR;
using Sudoku.Application.Common;
using Sudoku.Application.Queries;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class GetPlayerGamesQueryHandler : IQueryHandler<GetPlayerGamesQuery, List<GameDto>>
{
    private readonly IGameRepository _gameRepository;

    public GetPlayerGamesQueryHandler(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<Result<List<GameDto>>> Handle(GetPlayerGamesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Create player alias value object
            var playerAlias = PlayerAlias.Create(request.PlayerAlias);

            // Get all games for the player
            var games = await _gameRepository.GetByPlayerAsync(playerAlias);

            // Convert to DTOs
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