using System.Text.Json.Serialization;

namespace Sudoku.Web.Server.Models;

/// <summary>
/// Represents game statistics
/// </summary>
public class GameStatisticsModel
{
    public int TotalMoves { get; private set; }

    public int ValidMoves => TotalMoves - InvalidMoves;

    public int InvalidMoves { get; private set; }

    public TimeSpan PlayDuration { get; private set; }

    [JsonConstructor]
    public GameStatisticsModel(int totalMoves, int invalidMoves, TimeSpan playDuration)
    {
        TotalMoves = totalMoves;
        InvalidMoves = invalidMoves;
        PlayDuration = playDuration;
    }

    public GameStatisticsModel()
    {
    }

    public void RecordMove(bool isValid)
    {
        TotalMoves++;
        if (!isValid) InvalidMoves++;
    }

    public void Reset()
    {
        TotalMoves = 0;
        InvalidMoves = 0;
        PlayDuration = TimeSpan.Zero;
    }

    public void SetPlayDuration(TimeSpan duration) => PlayDuration = duration;
}