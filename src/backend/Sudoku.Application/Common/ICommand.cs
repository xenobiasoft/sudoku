using MediatR;

namespace Sudoku.Application.Common;

public interface ICommand : IRequest<Result>
{
}