using DepenMock.XUnit;
using Sudoku.Web.Server.Services;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Web.Services;

public class GameStorageManagerTests : BaseTestByAbstraction<GameStateManager, IGameStateManager>
{
    private readonly Mock<IGameStateStorage> _mockGameStateManager;
    private readonly Mock<ILocalStorageService> _mockLocalStorageService;

    public GameStorageManagerTests()
    {
        _mockGameStateManager = Container.ResolveMock<IGameStateStorage>();
        _mockLocalStorageService = Container.ResolveMock<ILocalStorageService>();
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
        _mockGameStateManager.VerifySaveAsyncCalled(Times.Once);
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
        _mockGameStateManager.VerifyLoadAsyncCalled(Times.Once);
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
    public async Task DeleteGameAsync_DeletesGameFromLocalStorage()
    {
        // Arrange
        var gameId = Container.Create<string>();
        _mockLocalStorageService.SetupLoadGameAsync(gameId, new GameStateMemory { PuzzleId = gameId });
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
        _mockLocalStorageService.SetupLoadGameAsync(gameId, new GameStateMemory { PuzzleId = gameId });
        var sut = ResolveSut();

        // Act
        await sut.DeleteGameAsync(gameId);

        // Assert
        _mockGameStateManager.VerifyDeleteAsyncCalled(Times.Once);
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
        _mockGameStateManager.VerifySaveAsyncCalled(Times.Once);
    }

    [Fact]
    public async Task UndoAsync_CallsSomething()
    {
        // Arrange
        var gameId = Container.Create<string>();
        var gameState = Container.Create<GameStateMemory>();
        var sut = ResolveSut();

        // Act
        await sut.UndoAsync(gameId);

        // Assert
        _mockGameStateManager.VerifyUndoAsyncCalled(Times.Once);
    }

    [Fact]
    public async Task UndoAsync_SavesNewGameStateToLocalStorage()
    {
        // Arrange
        var gameId = Container.Create<string>();
        var sut = ResolveSut();

        // Act
        var gameState = await sut.UndoAsync(gameId);

        // Assert
        _mockLocalStorageService.VerifySaveAsyncCalledWith(gameState, Times.Once);
    }
}