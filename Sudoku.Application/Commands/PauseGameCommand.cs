using Sudoku.Application.Common;

namespace Sudoku.Application.Commands;

public record PauseGameCommand(string GameId) : ICommand;