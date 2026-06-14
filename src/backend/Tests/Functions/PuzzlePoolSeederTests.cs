using DepenMock.Moq;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.ValueObjects;
using Sudoku.Functions.Services;

namespace UnitTests.Functions;

public class PuzzlePoolSeederTests : MoqBaseTestByAbstraction<PuzzlePoolSeeder, IPuzzlePoolSeeder>
{
    private readonly Mock<IPuzzlePoolService> _mockPuzzlePoolService;
    private const int TargetPoolSize = PuzzlePoolSeeder.TargetPoolSize;

    public PuzzlePoolSeederTests()
    {
        _mockPuzzlePoolService = Container.ResolveMock<IPuzzlePoolService>().AsMoq();
    }

    [Fact]
    public async Task SeedPoolAsync_WhenPoolBelowTarget_SeedsMissingPuzzlesForEachDifficulty()
    {
        // Arrange
        var currentCount = 7;
        var expectedSeedCount = TargetPoolSize - currentCount;
        _mockPuzzlePoolService.SetupGetAvailableCountReturns(currentCount);

        var sut = ResolveSut();

        // Act
        await sut.SeedPoolAsync();

        // Assert
        _mockPuzzlePoolService.VerifySeedCalledOnce(GameDifficulty.Easy, expectedSeedCount);
        _mockPuzzlePoolService.VerifySeedCalledOnce(GameDifficulty.Medium, expectedSeedCount);
        _mockPuzzlePoolService.VerifySeedCalledOnce(GameDifficulty.Hard, expectedSeedCount);
        _mockPuzzlePoolService.VerifySeedCalledOnce(GameDifficulty.Expert, expectedSeedCount);
    }

    [Fact]
    public async Task SeedPoolAsync_WhenPoolAtTarget_DoesNotSeedAnyPuzzles()
    {
        // Arrange
        _mockPuzzlePoolService.SetupGetAvailableCountReturns(TargetPoolSize);

        var sut = ResolveSut();

        // Act
        await sut.SeedPoolAsync();

        // Assert
        _mockPuzzlePoolService.VerifySeedNeverCalled();
    }

    [Fact]
    public async Task SeedPoolAsync_ChecksEachDifficultyExactlyOnce()
    {
        // Arrange
        _mockPuzzlePoolService.SetupGetAvailableCountReturns(TargetPoolSize);

        var sut = ResolveSut();

        // Act
        await sut.SeedPoolAsync();

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
    public async Task SeedPoolAsync_SeedsExactlyTheNumberNeededToReachTarget(int currentCount, int expectedSeedCount)
    {
        // Arrange
        _mockPuzzlePoolService.SetupGetAvailableCountReturns(currentCount);

        var sut = ResolveSut();

        // Act
        await sut.SeedPoolAsync();

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

    [Fact]
    public async Task SeedPoolAsync_ReturnsTotalSeededAcrossAllDifficulties()
    {
        // Arrange
        var currentCount = 7;
        var expectedTotal = (TargetPoolSize - currentCount) * 4; // four difficulties
        _mockPuzzlePoolService.SetupGetAvailableCountReturns(currentCount);

        var sut = ResolveSut();

        // Act
        var totalSeeded = await sut.SeedPoolAsync();

        // Assert
        totalSeeded.Should().Be(expectedTotal);
    }
}
