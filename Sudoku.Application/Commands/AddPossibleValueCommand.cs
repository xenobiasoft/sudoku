using Sudoku.Application.Common;

namespace Sudoku.Application.Commands;

public record AddPossibleValueCommand(
    string GameId,
    int Row,
    int Column,
    int Value) : ICommand;