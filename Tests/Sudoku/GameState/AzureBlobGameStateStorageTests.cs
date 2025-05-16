using DepenMock.XUnit;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Services;

namespace UnitTests.Sudoku.GameState;

public class AzureBlobGameStateStorageTests : BaseTestByAbstraction<AzureBlobGameStateStorage, IGameStateStorage<GameStateMemory>>
{
    private const string ContainerName = "sudoku-puzzles";
    private const string PuzzleId = "test-puzzle";

    private readonly Mock<IStorageService> _mockStorageService;
    private readonly GameStateMemory _gameState;
    private readonly List<string> _blobNames =
    [
        $"{PuzzleId}/00001.json",
        $"{PuzzleId}/00002.json",
        $"{PuzzleId}/00003.json"
    ];

    public AzureBlobGameStateStorageTests()
    {
        _mockStorageService = Container.ResolveMock<IStorageService>();
        _gameState = Container
            .Build<GameStateMemory>()
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
        await sut.DeleteAsync(PuzzleId);

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
    public async Task ResetAsync_ShouldCallResetGame()
    {
        // Arrange
        _mockStorageService.StubGetBlobNamesCall(_blobNames);
        var puzzleId = Container.Create<string>();
        var sut = ResolveSut();

        // Act
        await sut.ResetAsync(puzzleId);

        // Assert
        _mockStorageService.VerifyGetsBlobNames(ContainerName, puzzleId, Times.Once);
    }

    [Fact]
    public async Task ResetAsync_WhenOnInitialState_ShouldThrowException()
    {
        // Arrange
        _mockStorageService.StubGetBlobNamesCall([$"{PuzzleId}/00001.json"]);
        var sut = ResolveSut();

        // Act
        Task ResetGame() => sut.ResetAsync(PuzzleId);

        // Assert
        await Assert.ThrowsAsync<CannotResetInitialStateException>(ResetGame);
    }

    [Fact]
    public async Task ResetAsync_ShouldDeleteAllStatesExceptInitial()
    {
        // Arrange
        var deletedBlobNames = _blobNames.Skip(1);
        _mockStorageService.StubGetBlobNamesCall(_blobNames);
        var puzzleId = Container.Create<string>();
        var sut = ResolveSut();

        // Act
        await sut.ResetAsync(puzzleId);

        // Assert
        foreach (var blobName in deletedBlobNames)
        {
            _mockStorageService.VerifyDeletesBlob(ContainerName, blobName, Times.Once);
        }
    }

    [Fact]
    public async Task ResetAsync_ShouldReturnInitialGameState()
    {
        // Arrange
        var expected = _gameState;
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(_blobNames.First(), _gameState);
        var puzzleId = Container.Create<string>();
        var sut = ResolveSut();

        // Act
        var actual = await sut.ResetAsync(puzzleId);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task SaveAsync_ShouldLoadLatestGameState()
    {
        // Arrange
        _mockStorageService.StubGetBlobNamesCall(_blobNames);
        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(_gameState);

        // Assert
        _mockStorageService.VerifyLoadsGameState(ContainerName, _blobNames.Last(), Times.Once);
    }

    [Fact]
    public async Task SaveAsync_WhenNoPreviousGameState_ShouldSaveGameState()
    {
        // Arrange
        var expectedBlobName = $"{PuzzleId}/00001.json";
        _mockStorageService.StubGetBlobNamesCall([]);
        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(_gameState);

        // Assert
        _mockStorageService.VerifySavesGameState(ContainerName, expectedBlobName, _gameState, Times.Once);
    }

    [Fact]
    public async Task SaveAsync_WhenNewGameStateChanged_ShouldSaveGameState()
    {
        // Arrange
        var expectedBlobName = $"{PuzzleId}/00004.json";
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(_gameState);
        var gameState = Container
            .Build<GameStateMemory>()
            .With(x => x.PuzzleId, PuzzleId)
            .Create();
        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(gameState);

        // Assert
        _mockStorageService.VerifySavesGameState(ContainerName, expectedBlobName, gameState, Times.Once);
    }

    [Fact]
    public async Task SaveAsync_WhenNewGameStateUnchanged_ShouldNotSaveGameState()
    {
        // Arrange
        var expectedBlobName = $"{PuzzleId}/00004.json";
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(_gameState);
        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(_gameState);

        // Assert
        _mockStorageService.VerifySavesGameState(ContainerName, expectedBlobName, _gameState, Times.Never);
    }

    [Fact]
    public async Task UndoAsync_ShouldDeleteLatest()
    {
        // Arrange
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(_gameState);
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
            .StubLoadAsyncCall(_gameState);
        var sut = ResolveSut();

        // Act
        var result = await sut.UndoAsync(PuzzleId);

        // Assert
        result.Should().Be(_gameState);
    }

    [Fact]
    public async Task UndoAsync_WhenOnInitialState_ShouldThrowException()
    {
        // Arrange
        _mockStorageService.StubGetBlobNamesCall([$"{PuzzleId}/00001.json"]);
        var sut = ResolveSut();

        // Act
        Task UndoAsync() => sut.UndoAsync(PuzzleId);

        // Assert
        await Assert.ThrowsAsync<CannotUndoInitialStateException>(UndoAsync);
    }

    [Fact]
    public void GameStateMemoryType_ShouldBeAzureBlobPersistence()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var type = sut.MemoryType;

        // Assert
        type.Should().Be(GameStateMemoryType.AzureBlobPersistence);
    }
}