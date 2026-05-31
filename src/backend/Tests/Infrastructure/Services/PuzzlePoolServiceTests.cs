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
        var puzzles = new[]
        {
            PuzzleFactory.GetPuzzle(difficulty),
            PuzzleFactory.GetPuzzle(difficulty)
        };

        _mockBlobStorage
            .Setup(x => x.LoadAllAsync(difficulty.Name.ToLowerInvariant()))
            .ReturnsAsync(puzzles);

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
            .Setup(x => x.LoadAllAsync(difficulty.Name.ToLowerInvariant()))
            .ReturnsAsync([]);

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
    public async Task DequeueAsync_WithPuzzlesAvailable_ReturnsRandomPuzzleAndDeletesIt()
    {
        // Arrange
        var difficulty = GameDifficulty.Easy;
        var puzzle = PuzzleFactory.GetPuzzle(difficulty);
        var puzzles = new[] { puzzle };

        _mockBlobStorage
            .Setup(x => x.LoadAllAsync(difficulty.Name.ToLowerInvariant()))
            .ReturnsAsync(puzzles);

        _mockBlobStorage
            .Setup(x => x.DeleteAsync(difficulty.Name.ToLowerInvariant(), puzzle.PuzzleId))
            .Returns(Task.CompletedTask);

        var sut = ResolveSut();

        // Act
        var result = await sut.DequeueAsync(difficulty);

        // Assert
        result.Should().NotBeNull();
        result!.PuzzleId.Should().Be(puzzle.PuzzleId);
        _mockBlobStorage.Verify(
            x => x.DeleteAsync(difficulty.Name.ToLowerInvariant(), puzzle.PuzzleId),
            Times.Once);
    }

    [Fact]
    public async Task DequeueAsync_WithEmptyPool_ReturnsNull()
    {
        // Arrange
        var difficulty = GameDifficulty.Medium;

        _mockBlobStorage
            .Setup(x => x.LoadAllAsync(difficulty.Name.ToLowerInvariant()))
            .ReturnsAsync([]);

        var sut = ResolveSut();

        // Act
        var result = await sut.DequeueAsync(difficulty);

        // Assert
        result.Should().BeNull();
        _mockBlobStorage.Verify(
            x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }
}
