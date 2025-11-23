using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.Abstractions.V2;
using Sudoku.Web.Server.Services.States;

namespace Sudoku.Web.Server.Services.V2;

/// <summary>
/// Manages the lifecycle and state of a game session, including starting, pausing, resuming, and ending sessions, as
/// well as tracking game statistics and recording moves.
/// </summary>
/// <remarks>The <see cref="GameManager"/> class provides methods to control the flow of a game session and manage
/// its state. It also exposes the current game statistics through the <see cref="CurrentStatistics"/> property. This
/// class is designed to be used in scenarios where game session management and move tracking are required. Ensure that
/// the appropriate session lifecycle methods (e.g., <see cref="StartNewSession"/>, <see cref="PauseSession"/>,  <see
/// cref="ResumeSession"/>, and <see cref="EndSession"/>) are called in the correct order to maintain a valid session
/// state.</remarks>
public partial class GameManager : IGameStatisticsManager
{
    public IGameTimer Timer => gameTimer;

    /// <summary>
    /// Gets the current game statistics.
    /// </summary>
    public GameStatisticsModel CurrentStatistics => Game?.Statistics!;

    /// <summary>
    /// Ends the current session asynchronously.
    /// </summary>
    /// <remarks>This method terminates the active session and performs any necessary cleanup operations. 
    /// Ensure that all required session data is saved or processed before calling this method,  as the session will no
    /// longer be accessible after it completes.</remarks>
    /// <returns>A task that represents the asynchronous operation of ending the session.</returns>
    public async Task EndSession()
    {
        Game.Status = Game.IsSolved() ? GameStatus.Completed : GameStatus.Abandoned;
        gameTimer.OnTick -= OnTimerTick;
        gameTimer.Reset();
        await SaveGameAsync();
    }

    /// <summary>
    /// Pauses the current session asynchronously.
    /// </summary>
    /// <remarks>This method suspends the ongoing session, allowing it to be resumed later.  Ensure that a
    /// session is active before calling this method to avoid unexpected behavior.</remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task PauseSession()
    {
        Game.Status = GameStatus.Paused;
        gameTimer.Pause();
        await SaveGameAsync();
    }

    /// <summary>
    /// Records a move in the game and validates its state.
    /// </summary>
    /// <remarks>This method is intended to be used in scenarios where moves in a game need to be tracked  and
    /// their validity assessed. The specific behavior of the method depends on the implementation.</remarks>
    /// <param name="isValid">A boolean value indicating whether the move is valid.  <see langword="true"/> if the move is valid; otherwise,
    /// <see langword="false"/>.</param>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <param name="value"></param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task RecordMove(int row, int column, int? value, bool isValid)
    {
        CurrentStatistics.RecordMove(isValid);
        await SaveGameAsync(row, column, value);
    }

    /// <summary>
    /// Resumes a previously paused session asynchronously.
    /// </summary>
    /// <remarks>This method attempts to restore the state of a session that was previously paused.
    /// Ensure that a session exists and is in a paused state before calling this method.</remarks>
    /// <returns>A task that represents the asynchronous operation. The task completes when the session is successfully resumed.</returns>
    public Task ResumeSession()
    {
        Game.Status = GameStatus.InProgress;
        var playDuration = CurrentStatistics.PlayDuration;
        gameTimer.Resume(playDuration);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Starts a new session asynchronously.
    /// </summary>
    /// <remarks>This method initializes and begins a new session. Ensure that any required preconditions, such as
    /// proper configuration or initialization, are met before calling this method.</remarks>
    /// <returns>A task that represents the asynchronous operation of starting a new session.</returns>
    public async Task StartNewSession()
    {
        Game.Status = GameStatus.InProgress;
        CurrentStatistics.Reset();
        gameTimer.Reset();
        gameTimer.Start();
        gameTimer.OnTick += OnTimerTick;
        await SaveGameAsync();
    }

    private void OnTimerTick(object? sender, TimeSpan elapsedTime)
    {
        CurrentStatistics.SetPlayDuration(elapsedTime);
    }
}