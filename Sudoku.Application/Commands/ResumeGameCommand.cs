using Sudoku.Application.Common;

namespace Sudoku.Application.Commands;

public record ResumeGameCommand(string GameId) : ICommand;