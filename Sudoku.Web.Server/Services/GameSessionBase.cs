using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

/// <summary>
/// Base class for game sessions
/// </summary>
public abstract class GameSessionBase : IGameSession
{
    /// <summary>
    /// Gets whether this is a null session
    /// </summary>
    public abstract bool IsNull { get; }

    /// <summary>
    /// Gets the player's alias
    /// </summary>
    public abstract string Alias { get; }

    /// <summary>
    /// Gets the unique identifier of the puzzle
    /// </summary>
    public abstract string PuzzleId { get; }

    /// <summary>
    /// Gets the current state of the game board
    /// </summary>
    public abstract Cell[] Board { get; }

    /// <summary>
    /// Gets the number of invalid moves made
    /// </summary>
    public abstract int InvalidMoves { get; }

    /// <summary>
    /// Gets the total number of moves made
    /// </summary>
    public abstract int TotalMoves { get; }

    /// <summary>
    /// Gets the duration of the current game session
    /// </summary>
    public abstract TimeSpan PlayDuration { get; }

    /// <summary>
    /// Gets the timer for the game session
    /// </summary>
    public abstract IGameTimer Timer { get; }

    /// <summary>
    /// Event raised when a move is recorded
    /// </summary>
    public abstract event EventHandler? OnMoveRecorded;

    /// <summary>
    /// Records a move in the game
    /// </summary>
    /// <param name="isValid">Whether the move was valid</param>
    public abstract void RecordMove(bool isValid);

    /// <summary>
    /// Reloads the game board with a new state
    /// </summary>
    /// <param name="gameState">The new game state to load</param>
    public abstract void ReloadBoard(GameStateMemory gameState);

    /// <summary>
    /// Starts the game session
    /// </summary>
    public abstract void Start();

    /// <summary>
    /// Pauses the game session
    /// </summary>
    public abstract void Pause();

    /// <summary>
    /// Resumes the game session
    /// </summary>
    public abstract void Resume();

    /// <summary>
    /// Ends the game session
    /// </summary>
    public abstract void End();
}