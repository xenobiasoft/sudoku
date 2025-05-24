using DepenMock.XUnit;
using Sudoku.Web.Server.Components;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Services;

namespace UnitTests.Sudoku.GameState;

public class AzureBlobGameStateStorageTests : BaseTestByAbstraction<AzureBlobGameStateStorage, IGameStateStorage<GameStateMemory>>
{
    private const string ContainerName = "sudoku-puzzles";
    private const string Alias = "test-alias";
    private const string PuzzleId = "test-puzzle";

    private readonly Mock<IStorageService> _mockStorageService;
    private readonly GameStateMemory _gameState;
    private readonly List<string> _blobNames =
    [
        GetFullBlobName("00001"),
        GetFullBlobName("00002"),
        GetFullBlobName("00003")
    ];

    public AzureBlobGameStateStorageTests()
    {
        _mockStorageService = Container.ResolveMock<IStorageService>();
        _gameState = Container
            .Build<GameStateMemory>()
            .With(x => x.PuzzleId, PuzzleId)
            .With(x => x.Alias, Alias)
            .Create();
    }
    
    [Fact]
    public async Task DeleteAsync_WhenGivenAliasAndPuzzle_ShouldDeleteAllBlobs()
    {
        // Arrange
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(_gameState);
        var sut = ResolveSut();

        // Act
        await sut.DeleteAsync(Alias, PuzzleId);

        // Assert
        foreach (var blobName in _blobNames)
        {
            _mockStorageService.VerifyDeletesBlob(ContainerName, blobName, Times.Once);
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task DeleteAsync_WhenAliasIsEmpty_ThrowsException(string alias)
    {
        // Arrange
        var gameState = Container
            .Build<GameStateMemory>()
            .With(x => x.PuzzleId, PuzzleId)
            .With(x => x.Alias, alias)
            .Create();
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(gameState);
        var sut = ResolveSut();

        // Act
        var deleteAsync = sut.DeleteAsync(alias, PuzzleId);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(() => deleteAsync);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task DeleteAsync_WhenPuzzleIdIsEmpty_ThrowsException(string puzzleId)
    {
        // Arrange
        var gameState = Container
            .Build<GameStateMemory>()
            .With(x => x.PuzzleId, puzzleId)
            .With(x => x.Alias, Alias)
            .Create();
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(gameState);
        var sut = ResolveSut();

        // Act
        var deleteAsync = sut.DeleteAsync(Alias, puzzleId);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(() => deleteAsync);
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
        await sut.LoadAsync(Alias, PuzzleId);

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
        var result = await sut.LoadAsync(Alias, PuzzleId);

        // Assert
        result.Should().Be(_gameState);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task LoadAsync_WhenAliasIsEmpty_ThrowsException(string alias)
    {
        // Arrange
        var gameState = Container
            .Build<GameStateMemory>()
            .With(x => x.PuzzleId, PuzzleId)
            .With(x => x.Alias, alias)
            .Create();
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(gameState);
        var sut = ResolveSut();

        // Act
        var loadAsync = sut.LoadAsync(alias, PuzzleId);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(() => loadAsync);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task LoadAsync_WhenPuzzleIdIsEmpty_ThrowsException(string puzzleId)
    {
        // Arrange
        var gameState = Container
            .Build<GameStateMemory>()
            .With(x => x.PuzzleId, puzzleId)
            .With(x => x.Alias, Alias)
            .Create();
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(gameState);
        var sut = ResolveSut();

        // Act
        var loadAsync = sut.LoadAsync(Alias, puzzleId);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(() => loadAsync);
    }

    [Fact]
    public async Task ResetAsync_ShouldCallResetGame()
    {
        // Arrange
        var blobPrefix = GetBlobPrefix();
        _mockStorageService.StubGetBlobNamesCall(_blobNames);
        var sut = ResolveSut();

        // Act
        await sut.ResetAsync(Alias, PuzzleId);

        // Assert
        _mockStorageService.VerifyGetsBlobNames(ContainerName, blobPrefix, Times.Once);
    }

    [Fact]
    public async Task ResetAsync_WhenOnInitialState_ShouldThrowException()
    {
        // Arrange
        _mockStorageService.StubGetBlobNamesCall([GetFullBlobName("00001")]);
        var sut = ResolveSut();

        // Act
        Task ResetGame() => sut.ResetAsync(Alias, PuzzleId);

        // Assert
        await Assert.ThrowsAsync<CannotResetInitialStateException>(ResetGame);
    }

    [Fact]
    public async Task ResetAsync_ShouldDeleteAllStatesExceptInitial()
    {
        // Arrange
        var deletedBlobNames = _blobNames.Skip(1);
        _mockStorageService.StubGetBlobNamesCall(_blobNames);
        var sut = ResolveSut();

        // Act
        await sut.ResetAsync(Alias, PuzzleId);

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
        var sut = ResolveSut();

        // Act
        var actual = await sut.ResetAsync(Alias, PuzzleId);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task ResetAsync_WhenAliasIsEmpty_ThrowsException(string alias)
    {
        // Arrange
        var gameState = Container
            .Build<GameStateMemory>()
            .With(x => x.PuzzleId, PuzzleId)
            .With(x => x.Alias, alias)
            .Create();
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(gameState);
        var sut = ResolveSut();

        // Act
        var resetAsync = sut.ResetAsync(alias, PuzzleId);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(() => resetAsync);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task ResetAsync_WhenPuzzleIdIsEmpty_ThrowsException(string puzzleId)
    {
        // Arrange
        var gameState = Container
            .Build<GameStateMemory>()
            .With(x => x.PuzzleId, puzzleId)
            .With(x => x.Alias, Alias)
            .Create();
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(gameState);
        var sut = ResolveSut();

        // Act
        var resetAsync = sut.ResetAsync(Alias, puzzleId);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(() => resetAsync);
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
        var expectedBlobName = GetFullBlobName("00001");
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
        var expectedBlobName = GetFullBlobName("00004");
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(_gameState);
        var gameState = Container
            .Build<GameStateMemory>()
            .With(x => x.PuzzleId, PuzzleId)
            .With(x => x.Alias, Alias)
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
        var expectedBlobName = GetFullBlobName("00004");
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(_gameState);
        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(_gameState);

        // Assert
        _mockStorageService.VerifySavesGameState(ContainerName, expectedBlobName, _gameState, Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task SaveAsync_WhenAliasIsEmpty_ThrowsException(string alias)
    {
        // Arrange
        var gameState = Container
            .Build<GameStateMemory>()
            .With(x => x.PuzzleId, PuzzleId)
            .With(x => x.Alias, alias)
            .Create();
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(gameState);
        var sut = ResolveSut();

        // Act
        var saveAsync = sut.SaveAsync(gameState);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(() => saveAsync);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task SaveAsync_WhenPuzzleIdIsEmpty_ThrowsException(string puzzleId)
    {
        // Arrange
        var gameState = Container
            .Build<GameStateMemory>()
            .With(x => x.PuzzleId, puzzleId)
            .With(x => x.Alias, Alias)
            .Create();
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(gameState);
        var sut = ResolveSut();

        // Act
        var saveAsync = sut.SaveAsync(gameState);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(() => saveAsync);
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
        await sut.UndoAsync(Alias, PuzzleId);

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
        var result = await sut.UndoAsync(Alias, PuzzleId);

        // Assert
        result.Should().Be(_gameState);
    }

    [Fact]
    public async Task UndoAsync_WhenOnInitialState_ShouldThrowException()
    {
        // Arrange
        _mockStorageService.StubGetBlobNamesCall([GetFullBlobName("00001")]);
        var sut = ResolveSut();

        // Act
        Task UndoAsync() => sut.UndoAsync(Alias, PuzzleId);

        // Assert
        await Assert.ThrowsAsync<CannotUndoInitialStateException>(UndoAsync);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task UndoAsync_WhenAliasIsEmpty_ThrowsException(string alias)
    {
        // Arrange
        var gameState = Container
            .Build<GameStateMemory>()
            .With(x => x.PuzzleId, PuzzleId)
            .With(x => x.Alias, alias)
            .Create();
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(gameState);
        var sut = ResolveSut();

        // Act
        var undoAsync = sut.UndoAsync(alias, PuzzleId);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(() => undoAsync);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task UndoAsync_WhenPuzzleIdIsEmpty_ThrowsException(string puzzleId)
    {
        // Arrange
        var gameState = Container
            .Build<GameStateMemory>()
            .With(x => x.PuzzleId, puzzleId)
            .With(x => x.Alias, Alias)
            .Create();
        _mockStorageService
            .StubGetBlobNamesCall(_blobNames)
            .StubLoadAsyncCall(gameState);
        var sut = ResolveSut();

        // Act
        var undoAsync = sut.UndoAsync(Alias, puzzleId);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(() => undoAsync);
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

    private static string GetBlobPrefix()
    {
        return $"{Alias}/{PuzzleId}";
    }

    private static string GetFullBlobName(string gameName)
    {
        return $"{GetBlobPrefix()}/{gameName}.json";
    }
}