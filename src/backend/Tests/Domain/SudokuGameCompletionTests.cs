using DepenMock.Attributes;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.Events;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Factories;

namespace UnitTests.Domain;

[LogOutput(LogOutputTiming.Always)]
public class SudokuGameCompletionTests : MoqBaseTestByType<SudokuGame>
{
    [Fact]
    public void CompleteGame_WhenCalled_SetsStatusToCompleted()
    {
        // Arrange
        var sut = GameFactory.CreateGameWithDifficulty(GameDifficulty.Hard);

        // Act
        sut.CompleteGame();

        // Assert
        sut.Status.Should().Be(GameStatusEnum.Completed);
    }

    [Fact]
    public void CompleteGame_WhenCalled_RaisesGameCompletedEventWithTheProfileAndDifficulty()
    {
        // Arrange — the stats pipeline reads these off the event, so they must be carried on it.
        var sut = GameFactory.CreateGameWithDifficulty(GameDifficulty.Hard);

        // Act
        sut.CompleteGame();

        // Assert
        sut.DomainEvents.OfType<GameCompletedEvent>().Single()
            .Should().BeEquivalentTo(new
            {
                GameId = sut.Id,
                ProfileId = sut.ProfileId,
                Difficulty = GameDifficulty.Hard,
                Statistics = sut.Statistics
            });
    }

    [Fact]
    public void CompleteGame_WhenCalled_RaisesGameCompletedEventWhoseCompletedAtMatchesTheGame()
    {
        // Arrange
        var sut = GameFactory.CreateGameWithDifficulty(GameDifficulty.Easy);

        // Act
        sut.CompleteGame();

        // Assert
        sut.DomainEvents.OfType<GameCompletedEvent>().Single()
            .CompletedAt.Should().Be(sut.CompletedAt!.Value);
    }
}
