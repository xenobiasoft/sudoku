using MediatR;
using Sudoku.Application.Common;

namespace Sudoku.Application.Commands;

public record DeletePlayerGamesCommand(string PlayerAlias) : IRequest<Result>;

public class DeletePlayerGamesCommandHandler : IRequestHandler<DeletePlayerGamesCommand, Result>
{
    // This class will need to be implemented with the actual repository
    public Task<Result> Handle(DeletePlayerGamesCommand request, CancellationToken cancellationToken)
    {
        // Placeholder for actual implementation
        return Task.FromResult(Result.Success());
    }
}