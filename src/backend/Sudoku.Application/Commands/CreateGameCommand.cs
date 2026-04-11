using Sudoku.Application.Common;

namespace Sudoku.Application.Commands;

public record CreateGameCommand(
    string PlayerAlias,
    string Difficulty) : ICommand;