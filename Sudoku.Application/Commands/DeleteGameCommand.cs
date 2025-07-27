using MediatR;
using Sudoku.Application.Common;

namespace Sudoku.Application.Commands;

public record DeleteGameCommand(string GameId) : ICommand;

public class DeleteGameCommandHandler : IRequestHandler<DeleteGameCommand, Result>
{
    // This class will need to be implemented with the actual repository
    public Task<Result> Handle(DeleteGameCommand request, CancellationToken cancellationToken)
    {
        // Placeholder for actual implementation
        return Task.FromResult(Result.Success());
    }
}