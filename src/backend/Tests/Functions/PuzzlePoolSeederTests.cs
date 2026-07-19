using DepenMock.Attributes;
using DepenMock.Moq;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.ValueObjects;
using Sudoku.Functions.Services;

namespace UnitTests.Functions;

[LogOutput(LogOutputTiming.Always)]
public class PuzzlePoolSeederTests : MoqBaseTestByAbstraction<PuzzlePoolSeeder, IPuzzlePoolSeeder>
{
    private readonly Mock<IPuzzlePoolService> _mockPuzzlePoolService;
    private const int TargetPoolSizeNine = PuzzlePoolSeeder.TargetPoolSize;
    private const int TargetPoolSizeSixteen = PuzzlePoolSeeder.TargetPoolSizeSixteen;

    public PuzzlePoolSeederTests()
    {
        _mockPuzzlePoolService = Container.ResolveMock<IPuzzlePoolService>().AsMoq();
    }

    [Fact]
    public async Task SeedPoolAsync_WhenNineBySizePoolsBelowTarget_SeedsOnePuzzleAtATimeUntilTargetReached()
    {
        // Arrange — 16x16 pools already at target so only the four 9x9 combinations need seeding
        var currentCount = 8;
        var expectedPerCombination = TargetPoolSizeNine - currentCount;
        SetupAllCounts(nineCount: currentCount, sixteenCount: TargetPoolSizeSixteen);

        var sut = ResolveSut();

        // Act
        await sut.SeedPoolAsync();

        // Assert — one-at-a-time: SeedAsync(size, difficulty, 1) called once per missing puzzle
        _mockPuzzlePoolService.VerifySeedCalledTimes(BoardSize.Nine, GameDifficulty.Easy, 1, expectedPerCombination);
        _mockPuzzlePoolService.VerifySeedCalledTimes(BoardSize.Nine, GameDifficulty.Medium, 1, expectedPerCombination);
        _mockPuzzlePoolService.VerifySeedCalledTimes(BoardSize.Nine, GameDifficulty.Hard, 1, expectedPerCombination);
        _mockPuzzlePoolService.VerifySeedCalledTimes(BoardSize.Nine, GameDifficulty.Expert, 1, expectedPerCombination);
    }

    [Fact]
    public async Task SeedPoolAsync_WhenAllPoolsAtTarget_DoesNotSeedAnyPuzzles()
    {
        // Arrange
        SetupAllCounts(nineCount: TargetPoolSizeNine, sixteenCount: TargetPoolSizeSixteen);

        var sut = ResolveSut();

        // Act
        await sut.SeedPoolAsync();

        // Assert
        _mockPuzzlePoolService.VerifySeedNeverCalled();
    }

    [Fact]
    public async Task SeedPoolAsync_ChecksEachOfTheEightCombinationsExactlyOnce()
    {
        // Arrange
        SetupAllCounts(nineCount: TargetPoolSizeNine, sixteenCount: TargetPoolSizeSixteen);

        var sut = ResolveSut();

        // Act
        await sut.SeedPoolAsync();

        // Assert
        _mockPuzzlePoolService.VerifyGetAvailableCountCalledOnce(BoardSize.Nine, GameDifficulty.Easy);
        _mockPuzzlePoolService.VerifyGetAvailableCountCalledOnce(BoardSize.Nine, GameDifficulty.Medium);
        _mockPuzzlePoolService.VerifyGetAvailableCountCalledOnce(BoardSize.Nine, GameDifficulty.Hard);
        _mockPuzzlePoolService.VerifyGetAvailableCountCalledOnce(BoardSize.Nine, GameDifficulty.Expert);
        _mockPuzzlePoolService.VerifyGetAvailableCountCalledOnce(BoardSize.Sixteen, GameDifficulty.Easy);
        _mockPuzzlePoolService.VerifyGetAvailableCountCalledOnce(BoardSize.Sixteen, GameDifficulty.Medium);
        _mockPuzzlePoolService.VerifyGetAvailableCountCalledOnce(BoardSize.Sixteen, GameDifficulty.Hard);
        _mockPuzzlePoolService.VerifyGetAvailableCountCalledOnce(BoardSize.Sixteen, GameDifficulty.Expert);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(5, 5)]
    [InlineData(9, 1)]
    [InlineData(10, 0)]
    public async Task SeedPoolAsync_ForNineBySize_SeedsExactlyTheNumberNeededToReachTarget(int currentCount, int expectedSeedCount)
    {
        // Arrange — 16x16 pools already at target so only the 9x9 combination under test is exercised
        SetupAllCounts(nineCount: currentCount, sixteenCount: TargetPoolSizeSixteen);

        var sut = ResolveSut();

        // Act
        await sut.SeedPoolAsync();

        // Assert
        if (expectedSeedCount > 0)
        {
            _mockPuzzlePoolService.VerifySeedCalledTimes(BoardSize.Nine, GameDifficulty.Easy, 1, expectedSeedCount);
        }
        else
        {
            _mockPuzzlePoolService.VerifySeedNeverCalled();
        }
    }

    [Fact]
    public async Task SeedPoolAsync_ForSixteenBySixteen_TargetsFivePerDifficultyAndLeavesFullPoolsAlone()
    {
        // Arrange — 9x9 pools already full; only Sixteen/Easy is short by 2 puzzles
        SetupAllCounts(nineCount: TargetPoolSizeNine, sixteenCount: TargetPoolSizeSixteen);
        _mockPuzzlePoolService.SetupGetAvailableCountReturns(BoardSize.Sixteen, GameDifficulty.Easy, TargetPoolSizeSixteen - 2);

        var sut = ResolveSut();

        // Act
        await sut.SeedPoolAsync();

        // Assert
        _mockPuzzlePoolService.VerifySeedCalledTimes(BoardSize.Sixteen, GameDifficulty.Easy, 1, 2);
        _mockPuzzlePoolService.VerifySeedCalledTimes(BoardSize.Sixteen, GameDifficulty.Medium, 1, 0);
        _mockPuzzlePoolService.VerifySeedCalledTimes(BoardSize.Sixteen, GameDifficulty.Hard, 1, 0);
        _mockPuzzlePoolService.VerifySeedCalledTimes(BoardSize.Sixteen, GameDifficulty.Expert, 1, 0);
        _mockPuzzlePoolService.VerifySeedCalledTimes(BoardSize.Nine, GameDifficulty.Easy, 1, 0);
    }

    [Fact]
    public async Task SeedPoolAsync_ReturnsTotalSeededAcrossAllCombinations()
    {
        // Arrange
        var nineCurrentCount = 8;
        var expectedTotal = (TargetPoolSizeNine - nineCurrentCount) * 4;
        SetupAllCounts(nineCount: nineCurrentCount, sixteenCount: TargetPoolSizeSixteen);

        var sut = ResolveSut();

        // Act
        var totalSeeded = await sut.SeedPoolAsync();

        // Assert
        totalSeeded.Should().Be(expectedTotal);
    }

    private void SetupAllCounts(int nineCount, int sixteenCount)
    {
        _mockPuzzlePoolService.SetupGetAvailableCountReturns(BoardSize.Nine, GameDifficulty.Easy, nineCount);
        _mockPuzzlePoolService.SetupGetAvailableCountReturns(BoardSize.Nine, GameDifficulty.Medium, nineCount);
        _mockPuzzlePoolService.SetupGetAvailableCountReturns(BoardSize.Nine, GameDifficulty.Hard, nineCount);
        _mockPuzzlePoolService.SetupGetAvailableCountReturns(BoardSize.Nine, GameDifficulty.Expert, nineCount);
        _mockPuzzlePoolService.SetupGetAvailableCountReturns(BoardSize.Sixteen, GameDifficulty.Easy, sixteenCount);
        _mockPuzzlePoolService.SetupGetAvailableCountReturns(BoardSize.Sixteen, GameDifficulty.Medium, sixteenCount);
        _mockPuzzlePoolService.SetupGetAvailableCountReturns(BoardSize.Sixteen, GameDifficulty.Hard, sixteenCount);
        _mockPuzzlePoolService.SetupGetAvailableCountReturns(BoardSize.Sixteen, GameDifficulty.Expert, sixteenCount);
    }
}
