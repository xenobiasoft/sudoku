using Sudoku.Application.Common;
using Sudoku.Application.DTOs;

namespace Sudoku.Application.Commands;

public record UpdateProfileAliasCommand(string ProfileId, string NewAlias) : ICommand<ProfileDto>;
