using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services.Abstractions;

/// <summary>
/// Represents the properties of a game state
/// </summary>
public interface IGameStateProperties
{
    /// <summary>
    /// Gets the player's alias
    /// </summary>
    string Alias { get; }

    /// <summary>
    /// Gets the unique identifier of the puzzle
    /// </summary>
    string PuzzleId { get; }

    /// <summary>
    /// Gets the current state of the game board
    /// </summary>
    Cell[] Board { get; }

    /// <summary>
    /// Gets the number of invalid moves made
    /// </summary>
    int InvalidMoves { get; }

    /// <summary>
    /// Gets the total number of moves made
    /// </summary>
    int TotalMoves { get; }

    /// <summary>
    /// Gets the duration of the current game session
    /// </summary>
    TimeSpan PlayDuration { get; }

    /// <summary>
    /// Gets the timer for the game session
    /// </summary>
    IGameTimer Timer { get; }
}