using DepenMock.Moq;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Models;
using Sudoku.Infrastructure.Repositories;
using Sudoku.Infrastructure.Services;
using UnitTests.Helpers.Factories;

namespace UnitTests.Infrastructure.Repositories;

public class AzureBlobPuzzleRepositoryTests : MoqBaseTestByAbstraction<AzureBlobPuzzleRepository, IPuzzleBlobStorage>
{
    private readonly Mock<IAzureStorageService> _mockStorageService;
    private readonly Mock<IPuzzleGenerator> _mockPuzzleGenerator;
    private const string PuzzleContainer = "sudoku-puzzles";

    public AzureBlobPuzzleRepositoryTests()
    {
        _mockStorageService = Container.ResolveMock<IAzureStorageService>().AsMoq();
        _mockPuzzleGenerator = Container.ResolveMock<IPuzzleGenerator>().AsMoq();
    }

    [Fact]
    public async Task CreateAsync_GeneratesPuzzleSavesBlobAtCorrectPathAndReturnsPuzzle()
    {
        // Arrange
        var difficulty = GameDifficulty.Easy;
        var puzzle = PuzzleFactory.GetPuzzle(difficulty);

        _mockPuzzleGenerator
            .Setup(x => x.GeneratePuzzleAsync(difficulty))
            .ReturnsAsync(puzzle);

        var sut = (AzureBlobPuzzleRepository)ResolveSut();

        // Act
        var result = await sut.CreateAsync(difficulty);

        // Assert
        result.Should().NotBeNull();
        result.PuzzleId.Should().Be(puzzle.PuzzleId);
        _mockPuzzleGenerator.Verify(x => x.GeneratePuzzleAsync(difficulty), Times.Once);
        _mockStorageService.Verify(
            x => x.SaveAsync(
                PuzzleContainer,
                $"{difficulty.Name.ToLowerInvariant()}/{puzzle.PuzzleId}.json",
                It.IsAny<SudokuPuzzleDocument>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SavesBlobWithLowercaseDifficultyPrefix()
    {
        // Arrange
        var difficulty = GameDifficulty.Expert;
        var puzzle = PuzzleFactory.GetPuzzle(GameDifficulty.Easy);

        _mockPuzzleGenerator
            .Setup(x => x.GeneratePuzzleAsync(difficulty))
            .ReturnsAsync(puzzle);

        var sut = (AzureBlobPuzzleRepository)ResolveSut();

        // Act
        await sut.CreateAsync(difficulty);

        // Assert
        _mockStorageService.Verify(
            x => x.SaveAsync(
                PuzzleContainer,
                It.Is<string>(path => path.StartsWith("expert/")),
                It.IsAny<SudokuPuzzleDocument>()),
            Times.Once);
    }

    [Fact]
    public async Task GetPuzzleIdsAsync_WithBlobsInContainer_ReturnsIdsWithoutExtension()
    {
        // Arrange
        var prefix = "easy";
        var puzzleId = Guid.NewGuid().ToString();
        var blobName = $"{prefix}/{puzzleId}.json";

        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(PuzzleContainer, $"{prefix}/"))
            .Returns(new[] { blobName }.ToAsyncEnumerable());

        var sut = ResolveSut();

        // Act
        var ids = new List<string>();
        await foreach (var id in sut.GetPuzzleIdsAsync(prefix))
            ids.Add(id);

        // Assert
        ids.Should().ContainSingle().Which.Should().Be(puzzleId);
    }

    [Fact]
    public async Task GetPuzzleIdsAsync_WithEmptyContainer_ReturnsEmpty()
    {
        // Arrange
        var prefix = "medium";

        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(PuzzleContainer, $"{prefix}/"))
            .Returns(Array.Empty<string>().ToAsyncEnumerable());

        var sut = ResolveSut();

        // Act
        var ids = new List<string>();
        await foreach (var id in sut.GetPuzzleIdsAsync(prefix))
            ids.Add(id);

        // Assert
        ids.Should().BeEmpty();
    }

    [Fact]
    public async Task LoadAsync_WithExistingPuzzle_DeserializesAndReturnsPuzzle()
    {
        // Arrange
        var prefix = "hard";
        var puzzleId = Guid.NewGuid().ToString();
        var document = CreateEasyPuzzleDocument(puzzleId);

        _mockStorageService
            .Setup(x => x.LoadAsync<SudokuPuzzleDocument>(PuzzleContainer, $"{prefix}/{puzzleId}.json"))
            .ReturnsAsync(document);

        var sut = ResolveSut();

        // Act
        var result = await sut.LoadAsync(prefix, puzzleId);

        // Assert
        result.Should().NotBeNull();
        result!.PuzzleId.Should().Be(puzzleId);
    }

    [Fact]
    public async Task LoadAsync_WhenBlobMissing_ReturnsNull()
    {
        // Arrange
        var prefix = "easy";
        var puzzleId = Guid.NewGuid().ToString();

        _mockStorageService
            .Setup(x => x.LoadAsync<SudokuPuzzleDocument>(PuzzleContainer, $"{prefix}/{puzzleId}.json"))
            .ReturnsAsync((SudokuPuzzleDocument?)null);

        var sut = ResolveSut();

        // Act
        var result = await sut.LoadAsync(prefix, puzzleId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_CallsStorageDeleteWithCorrectBlobPath()
    {
        // Arrange
        var prefix = "hard";
        var puzzleId = Guid.NewGuid().ToString();
        var expectedBlobName = $"{prefix}/{puzzleId}.json";

        var sut = (AzureBlobPuzzleRepository)ResolveSut();

        // Act
        await sut.DeleteAsync(prefix, puzzleId);

        // Assert
        _mockStorageService.Verify(x => x.DeleteAsync(PuzzleContainer, expectedBlobName), Times.Once);
    }

    [Fact]
    public void CreateAsync_WithAlias_ThrowsNotSupportedException()
    {
        // Arrange
        var sut = (AzureBlobPuzzleRepository)ResolveSut();

        // Act
        var act = () => ((IPuzzleRepository)sut).CreateAsync("alias", GameDifficulty.Easy);

        // Assert
        act.Should().ThrowAsync<NotSupportedException>();
    }

    private static SudokuPuzzleDocument CreateEasyPuzzleDocument(string puzzleId)
    {
        var cells = new List<CellDocument>();
        for (var row = 0; row < 9; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                cells.Add(new CellDocument { Row = row, Column = col, Value = null, IsFixed = false });
            }
        }

        return new SudokuPuzzleDocument
        {
            PuzzleId = puzzleId,
            Difficulty = "easy",
            Cells = cells
        };
    }
}
