using Sudoku.Application.Common;

namespace Sudoku.Application.Commands;

public record CompleteGameCommand(string GameId) : ICommand;
