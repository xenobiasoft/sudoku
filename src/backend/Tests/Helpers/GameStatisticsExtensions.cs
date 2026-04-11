using Sudoku.Domain.ValueObjects;

namespace UnitTests.Helpers;

public static class GameStatisticsExtensions
{
    public static void AssertShouldBeEquivalent(this GameStatistics gameStats, GameStatistics expected)
    {
        Assert.Multiple(() =>
        {
            gameStats.AccuracyPercentage.Should().Be(expected.AccuracyPercentage);
            gameStats.HasMoves.Should().Be(expected.HasMoves);
            gameStats.InvalidMoves.Should().Be(expected.InvalidMoves);
            gameStats.PlayDuration.Should().Be(expected.PlayDuration);
            gameStats.TotalMoves.Should().Be(expected.TotalMoves);
            gameStats.ValidMoves.Should().Be(expected.ValidMoves);
        });
    }
}