using MediatR;
using Sudoku.Application.Common;

namespace Sudoku.Application.Commands;

public record DeletePlayerGamesCommand(string PlayerAlias) : ICommand;