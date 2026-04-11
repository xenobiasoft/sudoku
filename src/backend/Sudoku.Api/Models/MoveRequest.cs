namespace Sudoku.Api.Models;

public record MoveRequest(int Row, int Column, int? Value, TimeSpan PlayDuration);