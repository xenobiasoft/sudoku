using DepenMock.Moq;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Services;
using UnitTests.Helpers.Factories;

namespace UnitTests.Infrastructure.Services;

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

        _mockBlobStorage
            .Setup(x => x.GetPuzzleIdsAsync(difficulty.Name.ToLowerInvariant()))
            .Returns(new[] { "id1", "id2" }.ToAsyncEnumerable());

        var sut = ResolveSut();

        // Act
        var result = await sut.GetAvailableCountAsync(difficulty);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public async Task GetAvailableCountAsync_WithEmptyPool_ReturnsZero()
    {
        // Arrange
        var difficulty = GameDifficulty.Medium;

        _mockBlobStorage
            .Setup(x => x.GetPuzzleIdsAsync(difficulty.Name.ToLowerInvariant()))
            .Returns(Array.Empty<string>().ToAsyncEnumerable());

        var sut = ResolveSut();

        // Act
        var result = await sut.GetAvailableCountAsync(difficulty);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task SeedAsync_CallsCreateAsyncExactlyCountTimes()
    {
        // Arrange
        var difficulty = GameDifficulty.Hard;
        var count = 3;
        var puzzle = PuzzleFactory.GetPuzzle(difficulty);

        _mockBlobStorage
            .Setup(x => x.CreateAsync(difficulty))
            .ReturnsAsync(puzzle);

        var sut = ResolveSut();

        // Act
        await sut.SeedAsync(difficulty, count);

        // Assert
        _mockBlobStorage.Verify(x => x.CreateAsync(difficulty), Times.Exactly(count));
    }

    [Fact]
    public async Task SeedAsync_WithZeroCount_DoesNotCallCreateAsync()
    {
        // Arrange
        var difficulty = GameDifficulty.Expert;
        var sut = ResolveSut();

        // Act
        await sut.SeedAsync(difficulty, 0);

        // Assert
        _mockBlobStorage.Verify(x => x.CreateAsync(It.IsAny<GameDifficulty>()), Times.Never);
    }

    [Fact]
    public async Task DequeueAsync_WithPuzzleAvailable_LoadsOnlySelectedPuzzleAndDeletesIt()
    {
        // Arrange
        var difficulty = GameDifficulty.Easy;
        var puzzleId = Guid.NewGuid().ToString();
        var prefix = difficulty.Name.ToLowerInvariant();
        var puzzle = PuzzleFactory.GetPuzzle(difficulty);

        _mockBlobStorage
            .Setup(x => x.GetPuzzleIdsAsync(prefix))
            .Returns(new[] { puzzleId }.ToAsyncEnumerable());

        _mockBlobStorage
            .Setup(x => x.LoadAsync(prefix, puzzleId))
            .ReturnsAsync(puzzle);

        _mockBlobStorage
            .Setup(x => x.DeleteAsync(prefix, puzzleId))
            .Returns(Task.CompletedTask);

        var sut = ResolveSut();

        // Act
        var result = await sut.DequeueAsync(difficulty);

        // Assert
        result.Should().NotBeNull();
        result!.PuzzleId.Should().Be(puzzle.PuzzleId);
        _mockBlobStorage.Verify(x => x.LoadAsync(prefix, puzzleId), Times.Once);
        _mockBlobStorage.Verify(x => x.DeleteAsync(prefix, puzzleId), Times.Once);
    }

    [Fact]
    public async Task DequeueAsync_WithEmptyPool_ReturnsNullWithoutLoadOrDelete()
    {
        // Arrange
        var difficulty = GameDifficulty.Medium;

        _mockBlobStorage
            .Setup(x => x.GetPuzzleIdsAsync(difficulty.Name.ToLowerInvariant()))
            .Returns(Array.Empty<string>().ToAsyncEnumerable());

        var sut = ResolveSut();

        // Act
        var result = await sut.DequeueAsync(difficulty);

        // Assert
        result.Should().BeNull();
        _mockBlobStorage.Verify(x => x.LoadAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockBlobStorage.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DequeueAsync_WhenBlobVanishedBeforeLoad_ReturnsNull()
    {
        // Arrange
        var difficulty = GameDifficulty.Hard;
        var puzzleId = Guid.NewGuid().ToString();
        var prefix = difficulty.Name.ToLowerInvariant();

        _mockBlobStorage
            .Setup(x => x.GetPuzzleIdsAsync(prefix))
            .Returns(new[] { puzzleId }.ToAsyncEnumerable());

        _mockBlobStorage
            .Setup(x => x.LoadAsync(prefix, puzzleId))
            .ReturnsAsync((SudokuPuzzle?)null);

        var sut = ResolveSut();

        // Act
        var result = await sut.DequeueAsync(difficulty);

        // Assert
        result.Should().BeNull();
        _mockBlobStorage.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
