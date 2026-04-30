using Sudoku.Application.Common;
using Sudoku.Application.DTOs;

namespace Sudoku.Application.Commands;

public record CreateProfileCommand(string Alias) : ICommand<ProfileDto>;
