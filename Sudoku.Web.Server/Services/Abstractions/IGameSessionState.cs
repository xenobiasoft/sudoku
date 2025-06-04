using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services.Abstractions;

/// <summary>
/// Represents a game session state that combines properties and actions
/// </summary>
public interface IGameSessionState : IGameStateProperties, IGameStateActions
{
    /// <summary>
    /// Event raised when a move is recorded
    /// </summary>
    event EventHandler? OnMoveRecorded;
}