using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services.Abstractions;

/// <summary>
/// Represents a game session state
/// </summary>
public interface IGameSessionState
{
    /// <summary>
    /// Ends the game session
    /// </summary>
    void End();

    /// <summary>
    /// Pauses the game session
    /// </summary>
    void Pause();

    /// <summary>
    /// Records a move in the game
    /// </summary>
    /// <param name="isValid">Whether the move was valid</param>
    void RecordMove(bool isValid);

    /// <summary>
    /// Resumes the game session
    /// </summary>
    void Resume();

    /// <summary>
    /// Starts the game session
    /// </summary>
    void Start();
}