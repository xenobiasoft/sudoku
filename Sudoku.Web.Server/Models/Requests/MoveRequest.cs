namespace Sudoku.Web.Server.Models;

/// <summary>
/// Request model for making a move
/// </summary>
public record MoveRequest(int Row, int Column, int? Value);