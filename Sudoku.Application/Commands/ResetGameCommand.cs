using MediatR;
using Sudoku.Application.Common;

namespace Sudoku.Application.Commands;

public record ResetGameCommand(string GameId) : IRequest<Result>;

public class ResetGameCommandHandler : IRequestHandler<ResetGameCommand, Result>
{
    // This class will need to be implemented with the actual repository and domain logic
    public Task<Result> Handle(ResetGameCommand request, CancellationToken cancellationToken)
    {
        // Placeholder for actual implementation
        return Task.FromResult(Result.Success());
    }
}