using DepenMock.XUnit;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Services;

namespace UnitTests.Sudoku.GameState;

public class AzureStorageGameStateMemoryTests : BaseTestByAbstraction<AzureStorageGameStateMemory, IGameStateMemory>
{
    private const string ContainerName = "sudoku-puzzles";
    private const string PuzzleId = "test-puzzle";

    private readonly Mock<IStorageService> _mockStorageService;
    private readonly GameStateMemento _gameState;
    private readonly List<string> _blobNames =
    [
        $"{PuzzleId}/00001.json",
        $"{PuzzleId}/00002.json",
        $"{PuzzleId}/00003.json"
    ];

    public AzureStorageGameStateMemoryTests()
    {
        _mockStorageService = Container.ResolveMock<IStorageService>();
        _gameState = Container
            .Build<GameStateMemento>()
            .With(x => x.PuzzleId, PuzzleId)
            .Create();
    }
    
    [Fact]
    public async Task ClearAsync_ShouldDeleteAllBlobsForPuzzleId()
    {
        // Arrange
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(_gameState);
        var sut = ResolveSut();

        // Act
        await sut.ClearAsync(PuzzleId);

        // Assert
        foreach (var blobName in _blobNames)
        {
            _mockStorageService.VerifyDeletesBlob(ContainerName, blobName, Times.Once);
        }
    }

    [Fact]
    public async Task LoadAsync_ShouldCallLoadAsync()
    {
        // Arrange
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(_gameState);
        var sut = ResolveSut();

        // Act
        await sut.LoadAsync(PuzzleId);

        // Assert
        _mockStorageService.VerifyLoadsGameState(ContainerName, _blobNames.Last(), Times.Once);
    }

    [Fact]
    public async Task LoadAsync_ShouldReturnLatestGameState()
    {
        // Arrange
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(_gameState); ;
        var sut = ResolveSut();

        // Act
        var result = await sut.LoadAsync(PuzzleId);

        // Assert
        result.Should().Be(_gameState);
    }

    [Fact]
    public async Task SaveAsync_ShouldSaveGameState()
    {
        // Arrange
        var expectedBlobName = $"{PuzzleId}/00004.json";
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(_gameState); ;
        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(_gameState);

        // Assert
        _mockStorageService.VerifySavesGameState(ContainerName, expectedBlobName, _gameState, Times.Once);
    }

    [Fact]
    public async Task UndoAsync_ShouldDeleteLatest()
    {
        // Arrange
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(_gameState); ;
        var sut = ResolveSut();

        // Act
        await sut.UndoAsync(PuzzleId);

        // Assert
        _mockStorageService.VerifyDeletesBlob(ContainerName, _blobNames.Last(), Times.Once);
    }

    [Fact]
    public async Task UndoAsync_ShouldReturnPreviousGameState()
    {
        // Arrange
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(_gameState); ;
        var sut = ResolveSut();

        // Act
        var result = await sut.UndoAsync(PuzzleId);

        // Assert
        result.Should().Be(_gameState);
    }

    [Fact]
    public void GameStateMemoryType_ShouldBePersistence()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var type = sut.MemoryType;

        // Assert
        type.Should().Be(GameStateMemoryType.Persistence);
    }
}