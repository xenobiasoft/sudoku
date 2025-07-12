using Sudoku.Application.Common;

namespace Sudoku.Application.Commands;

public record MakeMoveCommand(
    string GameId,
    int Row,
    int Column,
    int Value) : ICommand;