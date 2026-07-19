using Sudoku.Application.Common;

namespace Sudoku.Application.Commands;

public record CreateGameCommand(
    string ProfileId,
    string DisplayName,
    string Difficulty,
    int Size = 9) : ICommand<string>;
