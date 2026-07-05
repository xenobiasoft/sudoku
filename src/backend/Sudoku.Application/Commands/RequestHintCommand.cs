using Sudoku.Application.Common;

namespace Sudoku.Application.Commands;

public record RequestHintCommand(string GameId, TimeSpan PlayDuration) : ICommand;
