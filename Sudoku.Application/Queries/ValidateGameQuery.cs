using MediatR;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;

namespace Sudoku.Application.Queries;

public record ValidateGameQuery(string GameId) : IRequest<Result<ValidationResultDto>>;

public class ValidateGameQueryHandler : IRequestHandler<ValidateGameQuery, Result<ValidationResultDto>>
{
    // This class will need to be implemented with the actual repository and domain logic
    public Task<Result<ValidationResultDto>> Handle(ValidateGameQuery request, CancellationToken cancellationToken)
    {
        // Placeholder for actual implementation
        var validationResult = new ValidationResultDto(
            IsValid: true,
            Errors: new List<string>()
        );
        
        return Task.FromResult(Result<ValidationResultDto>.Success(validationResult));
    }
}