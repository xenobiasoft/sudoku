using Sudoku.Application.Common;

namespace Sudoku.Application.Commands;

public record DeleteProfileCommand(string Alias) : ICommand;
