using Sudoku.Application.Common;
using Sudoku.Application.DTOs;

namespace Sudoku.Application.Queries;

public record GetGameQuery(string GameId) : IQuery<GameDto>;