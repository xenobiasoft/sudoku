using Sudoku.Application.Common;

namespace Sudoku.Application.Commands;

public record DeleteGameCommand(string GameId) : ICommand;