using DepenMock.XUnit;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers;

namespace UnitTests.Domain;

public class GameStatisticsTests : BaseTestByType<GameStatistics>
{
    [Fact]
    public void Create_ReturnsNewGameStatisticsWithInitialValues()
    {
        // Act
        var statistics = GameStatistics.Create();

        // Assert
        statistics.TotalMoves.Should().Be(0);
        statistics.ValidMoves.Should().Be(0);
        statistics.InvalidMoves.Should().Be(0);
        statistics.PlayDuration.Should().Be(TimeSpan.Zero);
        statistics.LastMoveAt.Should().BeNull();
        statistics.AccuracyPercentage.Should().Be(0);
        statistics.HasMoves.Should().BeFalse();
    }

    [Fact]
    public void RecordMove_WithValidMove_IncrementsValidMoves()
    {
        // Arrange
        var statistics = GameStatistics.Create();

        // Act
        statistics.RecordMove(true);

        // Assert
        statistics.TotalMoves.Should().Be(1);
        statistics.ValidMoves.Should().Be(1);
        statistics.InvalidMoves.Should().Be(0);
        statistics.LastMoveAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        statistics.HasMoves.Should().BeTrue();
    }

    [Fact]
    public void RecordMove_WithInvalidMove_IncrementsInvalidMoves()
    {
        // Arrange
        var statistics = GameStatistics.Create();

        // Act
        statistics.RecordMove(false);

        // Assert
        statistics.TotalMoves.Should().Be(1);
        statistics.ValidMoves.Should().Be(0);
        statistics.InvalidMoves.Should().Be(1);
        statistics.LastMoveAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        statistics.HasMoves.Should().BeTrue();
    }

    [Fact]
    public void RecordMove_MultipleMoves_UpdatesAllCounts()
    {
        // Arrange
        var statistics = GameStatistics.Create();

        // Act
        statistics.RecordMove(true);
        statistics.RecordMove(true);
        statistics.RecordMove(false);
        statistics.RecordMove(true);

        // Assert
        statistics.TotalMoves.Should().Be(4);
        statistics.ValidMoves.Should().Be(3);
        statistics.InvalidMoves.Should().Be(1);
        statistics.HasMoves.Should().BeTrue();
    }

    [Fact]
    public void UpdatePlayDuration_SetsPlayDuration()
    {
        // Arrange
        var statistics = GameStatistics.Create();
        var duration = TimeSpan.FromMinutes(10);

        // Act
        statistics.UpdatePlayDuration(duration);

        // Assert
        statistics.PlayDuration.Should().Be(duration);
    }

    [Fact]
    public void AccuracyPercentage_WithNoMoves_ReturnsZero()
    {
        // Arrange
        var statistics = GameStatistics.Create();

        // Act
        var accuracy = statistics.AccuracyPercentage;

        // Assert
        accuracy.Should().Be(0);
    }

    [Theory]
    [InlineData(5, 0, 0)]        // 0% accuracy
    [InlineData(5, 5, 100)]      // 100% accuracy
    [InlineData(10, 5, 50)]      // 50% accuracy
    [InlineData(4, 3, 75)]       // 75% accuracy
    public void AccuracyPercentage_CalculatesCorrectly(int totalMoves, int validMoves, double expectedAccuracy)
    {
        // Arrange
        var statistics = GameStatistics.Create();
        
        // Add valid moves
        for (int i = 0; i < validMoves; i++)
        {
            statistics.RecordMove(true);
        }
        
        // Add invalid moves (total - valid)
        for (int i = 0; i < totalMoves - validMoves; i++)
        {
            statistics.RecordMove(false);
        }

        // Act
        var accuracy = statistics.AccuracyPercentage;

        // Assert
        accuracy.Should().Be(expectedAccuracy);
    }

    [Fact]
    public void HasMoves_AfterRecordingMoves_ReturnsTrue()
    {
        // Arrange
        var statistics = GameStatistics.Create();
        statistics.RecordMove(true);

        // Act
        var hasMoves = statistics.HasMoves;

        // Assert
        hasMoves.Should().BeTrue();
    }

    [Fact]
    public void Equality_WithSameValues_AreEqual()
    {
        // Arrange
        var statistics1 = GameStatistics.Create();
        var statistics2 = GameStatistics.Create();

        statistics1.RecordMove(true);
        statistics1.UpdatePlayDuration(TimeSpan.FromMinutes(5));

        statistics2.RecordMove(true);
        statistics2.UpdatePlayDuration(TimeSpan.FromMinutes(5));
        
        // Force the LastMoveAt to be the same for testing equality
        var field = typeof(GameStatistics).GetField("LastMoveAt", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        var commonTime = DateTime.UtcNow;
        
        if (field != null)
        {
            field.SetValue(statistics1, commonTime);
            field.SetValue(statistics2, commonTime);
        }

        // Act & Assert
        statistics1.AssertShouldBeEquivalent(statistics2);
    }
}