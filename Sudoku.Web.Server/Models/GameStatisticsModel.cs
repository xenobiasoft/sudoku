namespace Sudoku.Web.Server.Models;

/// <summary>
/// Represents game statistics
/// </summary>
public record GameStatisticsModel(
    int TotalMoves,
    int ValidMoves,
    int InvalidMoves,
    TimeSpan PlayDuration,
    double AccuracyPercentage);