using Sudoku.Application.Common;
using Sudoku.Application.DTOs;

namespace Sudoku.Application.Queries;

public record GetPlayerStatsQuery(string ProfileId) : IQuery<PlayerStatsDto>;
