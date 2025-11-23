using Sudoku.Web.Server.Models;

namespace Sudoku.Web.Server.Services.Abstractions.V2;

public interface IGameStatisticsManager : IGameProvider
{
    IGameTimer Timer { get; }

    /// <summary>
    /// Gets the current game statistics, including metrics such as score, time played, and other relevant data.
    /// </summary>
    GameStatisticsModel CurrentStatistics { get; }

    /// <summary>
    /// Starts a new game session
    /// </summary>
    Task StartNewSession();

    /// <summary>
    /// Pauses the current game session
    /// </summary>
    Task PauseSession();

    /// <summary>
    /// Resumes the current game session
    /// </summary>
    Task ResumeSession();

    /// <summary>
    /// Ends the current game session
    /// </summary>
    Task EndSession();

    /// <summary>
    /// Records a move made by the player
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <param name="value"></param>
    /// <param name="isValid"></param>
    /// <returns></returns>
    Task RecordMove(int row, int column, int? value, bool isValid);
}