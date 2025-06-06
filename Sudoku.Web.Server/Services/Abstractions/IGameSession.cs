using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services.Abstractions;

/// <summary>
/// Represents a game session
/// </summary>
public interface IGameSession
{
    /// <summary>
    /// Gets whether this is a null session
    /// </summary>
    bool IsNull { get; }

    /// <summary>
    /// Gets the timer for the game session
    /// </summary>
    IGameTimer Timer { get; }

    /// <summary>
    /// Gets the current state of the game, including relevant game data and status.
    /// </summary>
    GameStateMemory GameState { get; }

    /// <summary>
    /// Gets or sets the current state of the game session.
    /// </summary>
    IGameSessionState SessionState { get; set; }

    /// <summary>
    /// Records a move in the game
    /// </summary>
    /// <param name="isValid">Whether the move was valid</param>
    void RecordMove(bool isValid);

    /// <summary>
    /// Ends the game session
    /// </summary>
    void End();

    /// <summary>
    /// Pauses the game session
    /// </summary>
    void Pause();

    /// <summary>
    /// Reloads the game board with a new state
    /// </summary>
    /// <param name="gameState">The new game state to load</param>
    void ReloadBoard(GameStateMemory gameState);

    /// <summary>
    /// Resumes the game session
    /// </summary>
    void Resume();

    /// <summary>
    /// Starts the game session
    /// </summary>
    void Start();
}