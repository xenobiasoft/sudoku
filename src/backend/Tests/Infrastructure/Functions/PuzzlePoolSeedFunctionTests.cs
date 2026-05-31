using DepenMock.Moq;
using Microsoft.Azure.Functions.Worker;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.ValueObjects;
using Sudoku.Functions.Functions;

namespace UnitTests.Infrastructure.Functions;

public class PuzzlePoolSeedFunctionTests : MoqBaseTestByType<PuzzlePoolSeedFunction>
{
    private readonly Mock<IPuzzlePoolService> _mockPuzzlePoolService;
    private const int TargetPoolSize = 10;

    public PuzzlePoolSeedFunctionTests()
    {
        _mockPuzzlePoolService = Container.ResolveMock<IPuzzlePoolService>().AsMoq();
    }

    [Fact]
    public async Task Run_WhenPoolBelowTarget_SeedsMissingPuzzlesForEachDifficulty()
    {
        // Arrange
        var currentCount = 7;
        var expectedSeedCount = TargetPoolSize - currentCount;

        _mockPuzzlePoolService
            .Setup(x => x.GetAvailableCountAsync(It.IsAny<GameDifficulty>()))
            .ReturnsAsync(currentCount);

        _mockPuzzlePoolService
            .Setup(x => x.SeedAsync(It.IsAny<GameDifficulty>(), expectedSeedCount))
            .Returns(Task.CompletedTask);

        var sut = ResolveSut();

        // Act
        await sut.Run(CreateTimerInfo());

        // Assert
        _mockPuzzlePoolService.Verify(
            x => x.SeedAsync(It.IsAny<GameDifficulty>(), expectedSeedCount),
            Times.Exactly(4)); // one for each difficulty
    }

    [Fact]
    public async Task Run_WhenPoolAtTarget_DoesNotSeedAnyPuzzles()
    {
        // Arrange
        _mockPuzzlePoolService
            .Setup(x => x.GetAvailableCountAsync(It.IsAny<GameDifficulty>()))
            .ReturnsAsync(TargetPoolSize);

        var sut = ResolveSut();

        // Act
        await sut.Run(CreateTimerInfo());

        // Assert
        _mockPuzzlePoolService.Verify(
            x => x.SeedAsync(It.IsAny<GameDifficulty>(), It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task Run_ChecksEachDifficultyExactlyOnce()
    {
        // Arrange
        _mockPuzzlePoolService
            .Setup(x => x.GetAvailableCountAsync(It.IsAny<GameDifficulty>()))
            .ReturnsAsync(TargetPoolSize);

        var sut = ResolveSut();

        // Act
        await sut.Run(CreateTimerInfo());

        // Assert
        _mockPuzzlePoolService.Verify(
            x => x.GetAvailableCountAsync(GameDifficulty.Easy), Times.Once);
        _mockPuzzlePoolService.Verify(
            x => x.GetAvailableCountAsync(GameDifficulty.Medium), Times.Once);
        _mockPuzzlePoolService.Verify(
            x => x.GetAvailableCountAsync(GameDifficulty.Hard), Times.Once);
        _mockPuzzlePoolService.Verify(
            x => x.GetAvailableCountAsync(GameDifficulty.Expert), Times.Once);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(5, 5)]
    [InlineData(9, 1)]
    [InlineData(10, 0)]
    public async Task Run_SeedsExactlyTheNumberNeededToReachTarget(int currentCount, int expectedSeedCount)
    {
        // Arrange
        _mockPuzzlePoolService
            .Setup(x => x.GetAvailableCountAsync(It.IsAny<GameDifficulty>()))
            .ReturnsAsync(currentCount);

        var sut = ResolveSut();

        // Act
        await sut.Run(CreateTimerInfo());

        // Assert
        if (expectedSeedCount > 0)
        {
            _mockPuzzlePoolService.Verify(
                x => x.SeedAsync(It.IsAny<GameDifficulty>(), expectedSeedCount),
                Times.Exactly(4));
        }
        else
        {
            _mockPuzzlePoolService.Verify(
                x => x.SeedAsync(It.IsAny<GameDifficulty>(), It.IsAny<int>()),
                Times.Never);
        }
    }

    private static TimerInfo CreateTimerInfo() => new();
}
