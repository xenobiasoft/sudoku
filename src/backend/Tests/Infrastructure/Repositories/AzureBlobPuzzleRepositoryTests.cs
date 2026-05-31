using DepenMock.Moq;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
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
    public async Task CreateAsync_SavesBlobWithCorrectDifficultyPrefix()
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
    public async Task LoadAllAsync_WithBlobsInContainer_DeserializesAndReturnsAllPuzzles()
    {
        // Arrange
        var alias = "easy";
        var puzzleId = Guid.NewGuid().ToString();
        var blobName = $"{alias}/{puzzleId}.json";
        var document = CreateEasyPuzzleDocument(puzzleId);

        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(PuzzleContainer, $"{alias}/"))
            .Returns(new[] { blobName }.ToAsyncEnumerable());

        _mockStorageService
            .Setup(x => x.LoadAsync<SudokuPuzzleDocument>(PuzzleContainer, blobName))
            .ReturnsAsync(document);

        var sut = ResolveSut();

        // Act
        var result = (await sut.LoadAllAsync(alias)).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].PuzzleId.Should().Be(puzzleId);
    }

    [Fact]
    public async Task LoadAllAsync_WithEmptyContainer_ReturnsEmptyCollection()
    {
        // Arrange
        var alias = "medium";

        _mockStorageService
            .Setup(x => x.GetBlobNamesAsync(PuzzleContainer, $"{alias}/"))
            .Returns(Array.Empty<string>().ToAsyncEnumerable());

        var sut = ResolveSut();

        // Act
        var result = await sut.LoadAllAsync(alias);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_CallsStorageDeleteWithCorrectBlobPath()
    {
        // Arrange
        var alias = "hard";
        var puzzleId = Guid.NewGuid().ToString();
        var expectedBlobName = $"{alias}/{puzzleId}.json";

        var sut = (AzureBlobPuzzleRepository)ResolveSut();

        // Act
        await sut.DeleteAsync(alias, puzzleId);

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
