using DepenMock.XUnit;
using Sudoku.Web.Server.Services;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Web.Services;

public class GameStateManagerTests : BaseTestByAbstraction<GameStateManager, IGameStateManager>
{
    private readonly Mock<IGameStateStorage<GameStateMemory>> _mockGameStateStorage;
    private readonly Mock<ILocalStorageService> _mockLocalStorageService;

    public GameStateManagerTests()
    {
        _mockGameStateStorage = Container.ResolveMock<IGameStateStorage<GameStateMemory>>();
        _mockLocalStorageService = Container.ResolveMock<ILocalStorageService>();
    }

    [Fact]
    public async Task DeleteGameAsync_DeletesGameFromLocalStorage()
    {
        // Arrange
        var gameId = Container.Create<string>();
        _mockLocalStorageService.SetupLoadGameAsync(gameId, new GameStateMemory(gameId, []));
        var sut = ResolveSut();

        // Act
        await sut.DeleteGameAsync(gameId);

        // Assert
        _mockLocalStorageService.VerifyDeleteGameAsyncCalled(Times.Once);
    }

    [Fact]
    public async Task DeleteGameAsync_DeletesGameFromStorageManager()
    {
        // Arrange
        var gameId = Container.Create<string>();
        _mockLocalStorageService.SetupLoadGameAsync(gameId, new GameStateMemory(gameId, []));
        var sut = ResolveSut();

        // Act
        await sut.DeleteGameAsync(gameId);

        // Assert
        _mockGameStateStorage.VerifyDeleteAsyncCalled(gameId, Times.Once);
    }

    [Fact]
    public async Task SaveGameAsync_CallsGameStateManager_SaveAsync()
    {
        // Arrange
        var gameState = Container.Create<GameStateMemory>();
        var sut = ResolveSut();

        // Act
        await sut.SaveGameAsync(gameState);

        // Assert
        _mockGameStateStorage.VerifySaveAsyncCalled(gameState, Times.Once);
    }

    [Fact]
    public async Task SaveGameAsync_CallsLocalStorageService_SaveGameStateAsync()
    {
        // Arrange
        var gameState = Container.Create<GameStateMemory>();
        var sut = ResolveSut();

        // Act
        await sut.SaveGameAsync(gameState);

        // Assert
        _mockLocalStorageService.VerifySaveGameAsyncCalled(Times.Once);
    }

    [Fact]
    public async Task LoadGameAsync_CallsLocalStorageService_LoadGameAsync()
    {
        // Arrange
        var gameId = Container.Create<string>();
        var sut = ResolveSut();

        // Act
        await sut.LoadGameAsync(gameId);

        // Assert
        _mockLocalStorageService.VerifyLoadGameAsyncCalled(Times.Once);
    }

    [Fact]
    public async Task LoadGameAsync_WhenGameNotExistInLocalStorage_LoadsGameFromGameStateManager()
    {
        // Arrange
        var gameId = Container.Create<string>();
        _mockLocalStorageService.SetupLoadGameAsync(gameId, null);
        var sut = ResolveSut();

        // Act
        await sut.LoadGameAsync(gameId);

        // Assert
        _mockGameStateStorage.VerifyLoadAsyncCalled(gameId, Times.Once);
    }

    [Fact]
    public async Task LoadGamesAsync_LoadsGamesFromGameStateManager()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        await sut.LoadGamesAsync();

        // Assert
        _mockLocalStorageService.VerifyLoadGamesAsyncCalled(Times.Once);
    }

    [Fact]
    public async Task ResetGameAsync_CallsGameStateStorageReset()
    {
        // Arrange
        var gameId = Container.Create<string>();
        var sut = ResolveSut();

        // Act
        await sut.ResetAsync(gameId);

        // Assert
        _mockGameStateStorage.VerifyResetAsyncCalled(gameId, Times.Once);
    }

    [Fact]
    public async Task SaveGameAsync_SavesGameInLocalStorage()
    {
        // Arrange
        var gameState = Container.Create<GameStateMemory>();
        var sut = ResolveSut();

        // Act
        await sut.SaveGameAsync(gameState);

        // Assert
        _mockLocalStorageService.VerifySaveGameAsyncCalled(Times.Once);
    }

    [Fact]
    public async Task SaveGameAsync_SavesGameInGameStorageManager()
    {
        // Arrange
        var gameState = Container.Create<GameStateMemory>();
        var sut = ResolveSut();

        // Act
        await sut.SaveGameAsync(gameState);

        // Assert
        _mockGameStateStorage.VerifySaveAsyncCalled(gameState, Times.Once);
    }

    [Fact]
    public async Task UndoAsync_CallsGameStateStorageUndo()
    {
        // Arrange
        var gameId = Container.Create<string>();
        var sut = ResolveSut();

        // Act
        await sut.UndoAsync(gameId);

        // Assert
        _mockGameStateStorage.VerifyUndoAsyncCalled(gameId, Times.Once);
    }

    [Fact]
    public async Task UndoAsync_WhenOnInitialGameState_DoesNotUndoGameState()
    {
        // Arrange
        var gameId = Container.Create<string>();
        var gameState = Container
            .Build<GameStateMemory>()
            .With(x => x.TotalMoves, 1)
            .Create();
        _mockLocalStorageService.SetupLoadGameAsync(gameId, gameState);
        var sut = ResolveSut();

        // Act
        await sut.UndoAsync(gameId);

        // Assert
        _mockGameStateStorage.VerifyUndoAsyncCalled(gameId, Times.Never);
    }
}