using Sudoku.Application.Common;

namespace Sudoku.Application.Commands;

public record AbandonGameCommand(string GameId) : ICommand;