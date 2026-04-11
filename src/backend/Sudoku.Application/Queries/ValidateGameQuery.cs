using Sudoku.Application.Common;
using Sudoku.Application.DTOs;

namespace Sudoku.Application.Queries;

public record ValidateGameQuery(string GameId) : IQuery<ValidationResultDto>;