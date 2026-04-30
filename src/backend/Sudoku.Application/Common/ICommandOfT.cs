using MediatR;

namespace Sudoku.Application.Common;

public interface ICommand<TResult> : IRequest<Result<TResult>> {}

public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, Result<TResult>>
    where TCommand : ICommand<TResult> {}
