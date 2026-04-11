using Microsoft.Extensions.Logging;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Queries;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class GetGameQueryHandler(IGameRepository gameRepository, ILogger<GetGameQueryHandler> logger) : IQueryHandler<GetGameQuery, GameDto>
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

            logger.LogInformation("Retrieved game {GameId}", gameId.Value);
            return Result<GameDto>.Success(gameDto);
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Failed to get game {GameId}: {Error}", request.GameId, ex.Message);
            return Result<GameDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred getting game {GameId}", request.GameId);
            return Result<GameDto>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}