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

    /// <summary>
    /// Event raised when a move is recorded
    /// </summary>
    event EventHandler? OnMoveRecorded;

    void ChangeState(IGameSessionState sessionState);

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
    /// Increments the count of invalid moves made by the user.
    /// </summary>
    /// <remarks>This method is typically used to track the number of invalid actions performed during a
    /// session or game. It updates an internal counter, which can be used for analytics, enforcing limits, or providing
    /// feedback to the user.</remarks>
    void IncrementInvalidMoves();

    /// <summary>
    /// Increments the total number of moves by one.
    /// </summary>
    /// <remarks>This method updates the internal counter tracking the total moves.  It is typically used to
    /// record progress in scenarios where moves are counted,  such as in games or simulations.</remarks>
    void IncrementTotalMoves();

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
    /// Reloads the game state from the specified memory source.
    /// </summary>
    /// <remarks>This method updates the current game state to match the state stored in the provided 
    /// <paramref name="gameState"/> object. Ensure that the game state memory is valid and  consistent before calling
    /// this method.</remarks>
    /// <param name="gameState">The memory object containing the game state to be reloaded.  This parameter cannot be <see langword="null"/>.</param>
    void ReloadGameState(GameStateMemory gameState);

    /// <summary>
    /// Resumes the game session
    /// </summary>
    void Resume();

    /// <summary>
    /// Starts the game session
    /// </summary>
    void Start();
}