using Sudoku.Application.Common;
using Sudoku.Application.DTOs;

namespace Sudoku.Application.Queries;

public record GetProfileByAliasQuery(string Alias) : IQuery<ProfileDto?>;
