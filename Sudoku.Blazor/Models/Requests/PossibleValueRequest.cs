namespace Sudoku.Blazor.Models.Requests;

/// <summary>
/// Request model for possible value operations
/// </summary>
public record PossibleValueRequest(int Row, int Column, int Value);