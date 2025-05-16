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
    public async Task LoadGameAsync_ShouldCallLocalStorageService_LoadGameAsync()
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
    public async Task DeleteGameAsync_ShouldDeleteGameFromGameStateStorage()
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
    public async Task DeleteGameAsync_ShouldDeleteGameFromLocalStorageService()
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
    public async Task LoadGamesAsync_ShouldLoadGamesFromGameStateStorage()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        await sut.LoadGamesAsync();

        // Assert
        _mockLocalStorageService.VerifyLoadGamesAsyncCalled(Times.Once);
    }

    [Fact]
    public async Task LoadGameAsync_WhenGameNotExistInLocalStorage_ShouldLoadGameFromGameStateStorage()
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
    public async Task ResetAsync_ShouldCallGameStateStorage_ResetAsync()
    {
        // Arrange
        var gameId = Container.Create<string>();
        var sut = ResolveSut();

        // Act
        await sut.ResetGameAsync(gameId);

        // Assert
        _mockGameStateStorage.VerifyResetAsyncCalled(gameId, Times.Once);
    }

    [Fact]
    public async Task ResetAsync_ShouldSaveGameInLocalStorageService()
    {
        // Arrange
        var gameId = Container.Create<string>();
        var sut = ResolveSut();

        // Act
        await sut.ResetGameAsync(gameId);

        // Assert
        _mockLocalStorageService.VerifySaveGameAsyncCalled(Times.Once);
    }

    [Fact]
    public async Task SaveGameAsync_ShouldCallGameStateStorage_SaveAsync()
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
    public async Task SaveGameAsync_ShouldCallLocalStorageService_SaveGameStateAsync()
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
    public async Task SaveGameAsync_ShouldSaveGameInGameStateStorage()
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
    public async Task SaveGameAsync_ShouldSaveGameInLocalStorageService()
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
    public async Task UndoAsync_ShouldCallGameStateStorage_UndoAsync()
    {
        // Arrange
        var gameId = Container.Create<string>();
        var sut = ResolveSut();

        // Act
        await sut.UndoGameAsync(gameId);

        // Assert
        _mockGameStateStorage.VerifyUndoAsyncCalled(gameId, Times.Once);
    }

    [Fact]
    public async Task UndoAsync_WhenOnInitialGameState_ShouldNotUndoGameState()
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
        await sut.UndoGameAsync(gameId);

        // Assert
        _mockGameStateStorage.VerifyUndoAsyncCalled(gameId, Times.Never);
    }
}