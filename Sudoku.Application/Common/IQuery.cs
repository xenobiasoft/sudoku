using MediatR;

namespace Sudoku.Application.Common;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}