using DepenMock.Attributes;
using DepenMock.Moq;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Services;
using UnitTests.Helpers.Factories;

namespace UnitTests.Infrastructure.Services;

[LogOutput(LogOutputTiming.Always)]
public class PuzzlePoolServiceTests : MoqBaseTestByAbstraction<PuzzlePoolService, IPuzzlePoolService>
{
    private readonly Mock<IPuzzleBlobStorage> _mockBlobStorage;

    public PuzzlePoolServiceTests()
    {
        _mockBlobStorage = Container.ResolveMock<IPuzzleBlobStorage>().AsMoq();
    }

    [Fact]
    public async Task GetAvailableCountAsync_WithPuzzlesInPool_ReturnsCorrectCount()
    {
        // Arrange
        var difficulty = GameDifficulty.Easy;

        _mockBlobStorage.SetupGetPuzzleIdsAsyncReturns(["id1", "id2"]);

        var sut = ResolveSut();

        // Act
        var result = await sut.GetAvailableCountAsync(BoardSize.Nine, difficulty);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public async Task GetAvailableCountAsync_WithEmptyPool_ReturnsZero()
    {
        // Arrange
        var difficulty = GameDifficulty.Medium;

        _mockBlobStorage.SetupGetPuzzleIdsAsyncReturnsEmpty();

        var sut = ResolveSut();

        // Act
        var result = await sut.GetAvailableCountAsync(BoardSize.Nine, difficulty);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task GetAvailableCountAsync_UsesSizeAndDifficultyPrefix()
    {
        // Arrange
        var difficulty = GameDifficulty.Hard;
        var expectedPrefix = "16x16/hard";

        _mockBlobStorage.SetupGetPuzzleIdsAsyncReturns(["id1"]);

        var sut = ResolveSut();

        // Act
        await sut.GetAvailableCountAsync(BoardSize.Sixteen, difficulty);

        // Assert
        _mockBlobStorage.VerifyGetPuzzleIdsAsyncCalledWith(expectedPrefix);
    }

    [Fact]
    public async Task SeedAsync_CallsCreateAsyncExactlyCountTimes()
    {
        // Arrange
        var difficulty = GameDifficulty.Hard;
        var count = 3;
        var puzzle = PuzzleFactory.GetPuzzle(difficulty);

        _mockBlobStorage.SetupCreateAsyncReturns(puzzle);

        var sut = ResolveSut();

        // Act
        await sut.SeedAsync(BoardSize.Nine, difficulty, count);

        // Assert
        _mockBlobStorage.VerifyCreateAsyncCalledExactly(difficulty, BoardSize.Nine, count);
    }

    [Fact]
    public async Task SeedAsync_WithZeroCount_DoesNotCallCreateAsync()
    {
        // Arrange
        var difficulty = GameDifficulty.Expert;
        var sut = ResolveSut();

        // Act
        await sut.SeedAsync(BoardSize.Nine, difficulty, 0);

        // Assert
        _mockBlobStorage.VerifyCreateAsyncNeverCalled();
    }

    [Fact]
    public async Task SeedAsync_ForSixteenBySixteen_PassesSizeToBlobStorage()
    {
        // Arrange
        var difficulty = GameDifficulty.Hard;
        var puzzle = PuzzleFactory.GetPuzzle(difficulty);

        _mockBlobStorage.SetupCreateAsyncReturns(puzzle);

        var sut = ResolveSut();

        // Act
        await sut.SeedAsync(BoardSize.Sixteen, difficulty, 1);

        // Assert
        _mockBlobStorage.VerifyCreateAsyncCalledExactly(difficulty, BoardSize.Sixteen, 1);
    }

    [Fact]
    public async Task DequeueAsync_WithPuzzleAvailable_LoadsOnlySelectedPuzzleAndDeletesIt()
    {
        // Arrange
        var difficulty = GameDifficulty.Easy;
        var puzzleId = Guid.NewGuid().ToString();
        var prefix = $"9x9/{difficulty.Name.ToLowerInvariant()}";
        var puzzle = PuzzleFactory.GetPuzzle(difficulty);

        _mockBlobStorage.SetupGetPuzzleIdsAsyncReturns([puzzleId]);
        _mockBlobStorage.SetupLoadAsyncReturns(puzzle);

        var sut = ResolveSut();

        // Act
        var result = await sut.DequeueAsync(BoardSize.Nine, difficulty);

        // Assert
        result.Should().NotBeNull();
        result!.PuzzleId.Should().Be(puzzle.PuzzleId);
        _mockBlobStorage.VerifyLoadAsyncCalledOnce(prefix, puzzleId);
        _mockBlobStorage.VerifyDeleteAsyncCalledOnce(prefix, puzzleId);
    }

    [Fact]
    public async Task DequeueAsync_WithEmptyPool_ReturnsNullWithoutLoadOrDelete()
    {
        // Arrange
        var difficulty = GameDifficulty.Medium;

        _mockBlobStorage.SetupGetPuzzleIdsAsyncReturnsEmpty();

        var sut = ResolveSut();

        // Act
        var result = await sut.DequeueAsync(BoardSize.Nine, difficulty);

        // Assert
        result.Should().BeNull();
        _mockBlobStorage.VerifyLoadAsyncNeverCalled();
        _mockBlobStorage.VerifyDeleteAsyncNeverCalled();
    }

    [Fact]
    public async Task DequeueAsync_WhenBlobVanishedBeforeLoad_ReturnsNull()
    {
        // Arrange
        var difficulty = GameDifficulty.Hard;
        var puzzleId = Guid.NewGuid().ToString();

        _mockBlobStorage.SetupGetPuzzleIdsAsyncReturns([puzzleId]);
        _mockBlobStorage.SetupLoadAsyncReturnsNull();

        var sut = ResolveSut();

        // Act
        var result = await sut.DequeueAsync(BoardSize.Nine, difficulty);

        // Assert
        result.Should().BeNull();
        _mockBlobStorage.VerifyDeleteAsyncNeverCalled();
    }

    [Fact]
    public async Task DequeueAsync_ForSixteenBySixteen_UsesSizePrefix()
    {
        // Arrange
        var difficulty = GameDifficulty.Expert;
        var puzzleId = Guid.NewGuid().ToString();
        var prefix = "16x16/expert";
        var puzzle = PuzzleFactory.GetPuzzle(difficulty);

        _mockBlobStorage.SetupGetPuzzleIdsAsyncReturns([puzzleId]);
        _mockBlobStorage.SetupLoadAsyncReturns(puzzle);

        var sut = ResolveSut();

        // Act
        var result = await sut.DequeueAsync(BoardSize.Sixteen, difficulty);

        // Assert
        result.Should().NotBeNull();
        _mockBlobStorage.VerifyLoadAsyncCalledOnce(prefix, puzzleId);
        _mockBlobStorage.VerifyDeleteAsyncCalledOnce(prefix, puzzleId);
    }
}
