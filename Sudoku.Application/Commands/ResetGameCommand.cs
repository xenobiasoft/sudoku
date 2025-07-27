using MediatR;
using Sudoku.Application.Common;

namespace Sudoku.Application.Commands;

public record ResetGameCommand(string GameId) : ICommand;