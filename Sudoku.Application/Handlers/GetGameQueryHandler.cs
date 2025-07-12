using MediatR;
using Sudoku.Application.Common;
using Sudoku.Application.Queries;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class GetGameQueryHandler : IQueryHandler<GetGameQuery, GameDto>
{
    private readonly IGameRepository _gameRepository;

    public GetGameQueryHandler(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<Result<GameDto>> Handle(GetGameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Parse the game ID
            var gameId = GameId.Create(request.GameId);

            // Get the game
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
                return Result<GameDto>.Failure($"Game not found with ID: {request.GameId}");

            // Convert to DTO
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