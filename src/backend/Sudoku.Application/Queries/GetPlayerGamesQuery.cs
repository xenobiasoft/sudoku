using Sudoku.Application.Common;
using Sudoku.Application.DTOs;

namespace Sudoku.Application.Queries;

public record GetPlayerGamesQuery(string PlayerAlias) : IQuery<List<GameDto>>;