namespace Sudoku.Web.Server.Services.Abstractions.V2;

public interface IGameStatistics
{
    public int TotalMoves { get; set; }
    public int ValidMoves { get; set; }
    public int InvalidMoves { get; set; }
    public TimeSpan PlayDuration { get; set; }
    public double AccuracyPercentage { get; set; }
}