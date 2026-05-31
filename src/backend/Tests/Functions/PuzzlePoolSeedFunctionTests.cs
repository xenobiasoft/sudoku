using DepenMock.Attributes;
using DepenMock.Moq;
using Microsoft.Azure.Functions.Worker;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.ValueObjects;
using Sudoku.Functions.Functions;

namespace UnitTests.Functions;

[LogOutput(LogOutputTiming.Always)]
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

        _mockPuzzlePoolService.SetupGetAvailableCountReturns(currentCount);

        var sut = ResolveSut();

        // Act
        await sut.Run(CreateTimerInfo());

        // Assert
        _mockPuzzlePoolService.VerifySeedCalledOnce(GameDifficulty.Easy, expectedSeedCount);
        _mockPuzzlePoolService.VerifySeedCalledOnce(GameDifficulty.Medium, expectedSeedCount);
        _mockPuzzlePoolService.VerifySeedCalledOnce(GameDifficulty.Hard, expectedSeedCount);
        _mockPuzzlePoolService.VerifySeedCalledOnce(GameDifficulty.Expert, expectedSeedCount);
    }

    [Fact]
    public async Task Run_WhenPoolAtTarget_DoesNotSeedAnyPuzzles()
    {
        // Arrange
        _mockPuzzlePoolService.SetupGetAvailableCountReturns(TargetPoolSize);

        var sut = ResolveSut();

        // Act
        await sut.Run(CreateTimerInfo());

        // Assert
        _mockPuzzlePoolService.VerifySeedNeverCalled();
    }

    [Fact]
    public async Task Run_ChecksEachDifficultyExactlyOnce()
    {
        // Arrange
        _mockPuzzlePoolService.SetupGetAvailableCountReturns(TargetPoolSize);

        var sut = ResolveSut();

        // Act
        await sut.Run(CreateTimerInfo());

        // Assert
        _mockPuzzlePoolService.VerifyGetAvailableCountCalledOnce(GameDifficulty.Easy);
        _mockPuzzlePoolService.VerifyGetAvailableCountCalledOnce(GameDifficulty.Medium);
        _mockPuzzlePoolService.VerifyGetAvailableCountCalledOnce(GameDifficulty.Hard);
        _mockPuzzlePoolService.VerifyGetAvailableCountCalledOnce(GameDifficulty.Expert);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(5, 5)]
    [InlineData(9, 1)]
    [InlineData(10, 0)]
    public async Task Run_SeedsExactlyTheNumberNeededToReachTarget(int currentCount, int expectedSeedCount)
    {
        // Arrange
        _mockPuzzlePoolService.SetupGetAvailableCountReturns(currentCount);

        var sut = ResolveSut();

        // Act
        await sut.Run(CreateTimerInfo());

        // Assert
        if (expectedSeedCount > 0)
        {
            _mockPuzzlePoolService.VerifySeedCalledOnce(GameDifficulty.Easy, expectedSeedCount);
            _mockPuzzlePoolService.VerifySeedCalledOnce(GameDifficulty.Medium, expectedSeedCount);
            _mockPuzzlePoolService.VerifySeedCalledOnce(GameDifficulty.Hard, expectedSeedCount);
            _mockPuzzlePoolService.VerifySeedCalledOnce(GameDifficulty.Expert, expectedSeedCount);
        }
        else
        {
            _mockPuzzlePoolService.VerifySeedNeverCalled();
        }
    }

    private static TimerInfo CreateTimerInfo() => new();
}
