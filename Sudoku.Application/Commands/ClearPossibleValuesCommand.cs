using Sudoku.Application.Common;

namespace Sudoku.Application.Commands;

public record ClearPossibleValuesCommand(
    string GameId,
    int Row,
    int Column) : ICommand;