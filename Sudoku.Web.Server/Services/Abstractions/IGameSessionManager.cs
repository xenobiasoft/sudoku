using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services.Abstractions;

/// <summary>
/// Manages game sessions and their persistence
/// </summary>
public interface IGameSessionManager
{
    /// <summary>
    /// Gets the current game session
    /// </summary>
    IGameSession CurrentSession { get; }

    /// <summary>
    /// Starts a new game session
    /// </summary>
    /// <param name="gameState">The initial game state</param>
    Task StartNewSession(GameStateMemory gameState);

    /// <summary>
    /// Pauses the current game session
    /// </summary>
    Task PauseSession();

    /// <summary>
    /// Resumes the current game session
    /// </summary>
    /// <param name="gameState">The game state to resume with</param>
    Task ResumeSession(GameStateMemory gameState);

    /// <summary>
    /// Ends the current game session
    /// </summary>
    Task EndSession();

    /// <summary>
    /// Records a move in the current game session
    /// </summary>
    /// <param name="isValid">Whether the move was valid</param>
    Task RecordMove(bool isValid);
}