namespace Sudoku.Web.Server.Models;

/// <summary>
/// Request model for creating a player
/// </summary>
public record CreatePlayerRequest(string? Alias);