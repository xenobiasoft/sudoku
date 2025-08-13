namespace Sudoku.Api.Models;

public record PossibleValueRequest(int Row, int Column, int Value);