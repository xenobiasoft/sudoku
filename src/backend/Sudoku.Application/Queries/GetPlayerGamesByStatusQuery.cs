using Sudoku.Application.Common;
using Sudoku.Application.DTOs;

namespace Sudoku.Application.Queries;

public record GetPlayerGamesByStatusQuery(
    string ProfileId,
    string Status) : IQuery<List<GameDto>>;
