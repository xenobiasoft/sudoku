using MediatR;
using Sudoku.Application.Common;
using Sudoku.Application.Queries;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class GetPlayerGamesByStatusQueryHandler : IQueryHandler<GetPlayerGamesByStatusQuery, List<GameDto>>
{
    private readonly IGameRepository _gameRepository;

    public GetPlayerGamesByStatusQueryHandler(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<Result<List<GameDto>>> Handle(GetPlayerGamesByStatusQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Create player alias value object
            var playerAlias = PlayerAlias.Create(request.PlayerAlias);

            // Parse game status
            if (!Enum.TryParse<GameStatus>(request.Status, true, out var gameStatus))
                return Result<List<GameDto>>.Failure($"Invalid game status: {request.Status}");

            // Get games for the player with the specified status
            var games = await _gameRepository.GetByPlayerAndStatusAsync(playerAlias, gameStatus);

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