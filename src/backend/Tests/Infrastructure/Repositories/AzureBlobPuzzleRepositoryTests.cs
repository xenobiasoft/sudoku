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

    private readonly IPuzzleBlobStorage _sut;

    public AzureBlobPuzzleRepositoryTests()
    {
        _mockStorageService = Container.ResolveMock<IAzureStorageService>().AsMoq();
        _mockPuzzleGenerator = Container.ResolveMock<IPuzzleGenerator>().AsMoq();

        _sut = ResolveSut();
    }

    [Fact]
    public async Task CreateAsync_GeneratesPuzzleSavesBlobAtCorrectPathAndReturnsPuzzle()
    {
        // Arrange
        var difficulty = GameDifficulty.Easy;
        var puzzle = PuzzleFactory.GetPuzzle(difficulty);
        var expectedBlobName = $"{difficulty.Name.ToLowerInvariant()}/{puzzle.PuzzleId}.json";

        _mockPuzzleGenerator
            .Setup(x => x.GeneratePuzzleAsync(difficulty))
            .ReturnsAsync(puzzle);

        // Act
        var result = await _sut.CreateAsync(difficulty);

        // Assert
        result.Should().NotBeNull();
        result.PuzzleId.Should().Be(puzzle.PuzzleId);
        _mockPuzzleGenerator.VerifyGeneratePuzzleAsyncCalledOnce(difficulty);
        _mockStorageService.VerifySavesBlob<SudokuPuzzleDocument>(PuzzleContainer, expectedBlobName);
    }

    [Fact]
    public async Task CreateAsync_SavesBlobWithLowercaseDifficultyPrefix()
    {
        // Arrange
        var difficulty = GameDifficulty.Expert;
        var puzzle = PuzzleFactory.GetPuzzle(GameDifficulty.Easy);
        _mockPuzzleGenerator.SetupGeneratePuzzleAsyncReturns(puzzle);

        // Act
        await _sut.CreateAsync(difficulty);

        // Assert
        _mockStorageService.VerifySavesBlobCheckUsingPartialName<SudokuPuzzleDocument>(PuzzleContainer, "expert/");
    }

    [Fact]
    public async Task GetPuzzleIdsAsync_WithBlobsInContainer_ReturnsIdsWithoutExtension()
    {
        // Arrange
        var expectedIds = new List<string>();
        var prefix = "easy";
        var puzzleId = Guid.NewGuid().ToString();
        var blobName = $"{prefix}/{puzzleId}.json";

        _mockStorageService.SetupGetBlobNamesReturns([blobName]);

        // Act
        await foreach (var id in _sut.GetPuzzleIdsAsync(prefix))
        {
            expectedIds.Add(id);
        }

        // Assert
        expectedIds.Should().ContainSingle().Which.Should().Be(puzzleId);
    }

    [Fact]
    public async Task GetPuzzleIdsAsync_WithEmptyContainer_ReturnsEmpty()
    {
        // Arrange
        var expectedIds = new List<string>();
        var prefix = "medium";
        _mockStorageService.SetupGetBlobNamesReturns([]);

        // Act
        await foreach (var id in _sut.GetPuzzleIdsAsync(prefix))
        {
            expectedIds.Add(id);
        }

        // Assert
        expectedIds.Should().BeEmpty();
    }

    [Fact]
    public async Task LoadAsync_WithExistingPuzzle_DeserializesAndReturnsPuzzle()
    {
        // Arrange
        var prefix = "hard";
        var puzzleId = Guid.NewGuid().ToString();
        var document = CreateEasyPuzzleDocument(puzzleId);

        _mockStorageService.SetupLoadReturns(document);

        // Act
        var result = await _sut.LoadAsync(prefix, puzzleId);

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
        _mockStorageService.SetupLoadReturns<SudokuPuzzleDocument>(null!);

        // Act
        var result = await _sut.LoadAsync(prefix, puzzleId);

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

        // Act
        await _sut.DeleteAsync(prefix, puzzleId);

        // Assert
        _mockStorageService.VerifyDeletesBlob(PuzzleContainer, expectedBlobName);
    }

    [Fact]
    public void CreateAsync_WithAlias_ThrowsNotSupportedException()
    {
        // Arrange

        // Act
        var act = () => ((IPuzzleRepository)_sut).CreateAsync("alias", GameDifficulty.Easy);

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
