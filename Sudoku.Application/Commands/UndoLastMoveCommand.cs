using MediatR;
using Sudoku.Application.Common;

namespace Sudoku.Application.Commands;

public record UndoLastMoveCommand(string GameId) : ICommand;