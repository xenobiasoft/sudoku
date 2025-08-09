namespace Sudoku.Api.Models;

/// <summary>
/// Request model for creating a player
/// </summary>
public class CreatePlayerRequest
{
    /// <summary>
    /// The player's alias. If not provided, a random alias will be generated.
    /// </summary>
    public string? Alias { get; set; }
}