using Microsoft.Extensions.Logging;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Queries;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class ValidateGameQueryHandler(IGameRepository gameRepository, ILogger<ValidateGameQueryHandler> logger) : IQueryHandler<ValidateGameQuery, ValidationResultDto>
{
    public async Task<Result<ValidationResultDto>> Handle(ValidateGameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var gameId = GameId.Create(request.GameId);
            
            var game = await gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                return Result<ValidationResultDto>.Failure($"Game not found with ID: {request.GameId}");
            }
            
            var validationResult = game.ValidateGame();
            
            var resultDto = new ValidationResultDto(
                IsValid: validationResult.IsValid,
                Errors: validationResult.Errors
            );
            
            logger.LogInformation("Validated game with ID: {GameId}, IsValid: {IsValid}", gameId.Value, validationResult.IsValid);
            return Result<ValidationResultDto>.Success(resultDto);
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Failed to validate game: {Error}", ex.Message);
            return Result<ValidationResultDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred validating game");
            return Result<ValidationResultDto>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }
}