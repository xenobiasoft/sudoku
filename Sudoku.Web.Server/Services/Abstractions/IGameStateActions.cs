using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services.Abstractions;

/// <summary>
/// Represents the actions that can be performed on a game state
/// </summary>
public interface IGameStateActions
{
    /// <summary>
    /// Records a move in the game
    /// </summary>
    /// <param name="isValid">Whether the move was valid</param>
    void RecordMove(bool isValid);

    /// <summary>
    /// Reloads the game board with a new state
    /// </summary>
    /// <param name="gameState">The new game state to load</param>
    void ReloadBoard(GameStateMemory gameState);

    /// <summary>
    /// Starts the game session
    /// </summary>
    void Start();

    /// <summary>
    /// Pauses the game session
    /// </summary>
    void Pause();

    /// <summary>
    /// Resumes the game session
    /// </summary>
    void Resume();

    /// <summary>
    /// Ends the game session
    /// </summary>
    void End();
}