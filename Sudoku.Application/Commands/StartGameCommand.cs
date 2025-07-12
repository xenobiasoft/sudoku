using Sudoku.Application.Common;

namespace Sudoku.Application.Commands;

public record StartGameCommand(string GameId) : ICommand;