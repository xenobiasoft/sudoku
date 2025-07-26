using MediatR;
using Sudoku.Application.Common;

namespace Sudoku.Application.Commands;

public record UndoLastMoveCommand(string GameId) : IRequest<Result>;

public class UndoLastMoveCommandHandler : IRequestHandler<UndoLastMoveCommand, Result>
{
    // This class will need to be implemented with the actual repository and domain logic
    public Task<Result> Handle(UndoLastMoveCommand request, CancellationToken cancellationToken)
    {
        // Placeholder for actual implementation
        return Task.FromResult(Result.Success());
    }
}