using Sudoku.Application.Common;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Commands;

public record CreateGameCommand(
    string PlayerAlias,
    string Difficulty) : ICommand;