using DepenMock.XUnit;
using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.Abstractions;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Web.Services;

public class GameStateManagerTests : BaseTestByAbstraction<GameStateManager, IGameStateManager>
{
    private const string Alias = "TestAlias";
    private const string GameId = "TestGameId";

    private readonly Mock<IPersistentGameStateStorage> _mockGameStateStorage;
    private readonly Mock<ILocalStorageService> _mockLocalStorageService;

    public GameStateManagerTests()
    {
        _mockGameStateStorage = Container.ResolveMock<IPersistentGameStateStorage>();
        _mockLocalStorageService = Container.ResolveMock<ILocalStorageService>();
    }

    [Fact]
    public async Task LoadGameAsync_ShouldCallLocalStorageService_LoadGameAsync()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        await sut.LoadGameAsync(Alias, GameId);

        // Assert
        _mockLocalStorageService.VerifyLoadGameAsyncCalled(Times.Once);
    }

    [Fact]
    public async Task DeleteGameAsync_ShouldDeleteGameFromGameStateStorage()
    {
        // Arrange
        _mockLocalStorageService.SetupLoadGameAsync(new GameStateMemory { Board = [], PuzzleId = GameId});
        var sut = ResolveSut();

        // Act
        await sut.DeleteGameAsync(Alias, GameId);

        // Assert
        _mockGameStateStorage.VerifyDeleteAsyncCalled(Alias, GameId, Times.Once);
    }

    [Fact]
    public async Task DeleteGameAsync_ShouldDeleteGameFromLocalStorageService()
    {
        // Arrange
        _mockLocalStorageService.SetupLoadGameAsync(new GameStateMemory { Board = [], PuzzleId = GameId });
        var sut = ResolveSut();

        // Act
        await sut.DeleteGameAsync(Alias, GameId);

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
        _mockLocalStorageService.SetupLoadGameAsync(null);
        var sut = ResolveSut();

        // Act
        await sut.LoadGameAsync(Alias, GameId);

        // Assert
        _mockGameStateStorage.VerifyLoadAsyncCalled(Alias, GameId, Times.Once);
    }

    [Fact]
    public async Task ResetAsync_ShouldCallGameStateStorage_ResetAsync()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        await sut.ResetGameAsync(Alias, GameId);

        // Assert
        _mockGameStateStorage.VerifyResetAsyncCalled(Alias, GameId, Times.Once);
    }

    [Fact]
    public async Task ResetAsync_ShouldSaveGameInLocalStorageService()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        await sut.ResetGameAsync(Alias, GameId);

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
        var sut = ResolveSut();

        // Act
        await sut.UndoGameAsync(Alias, GameId);

        // Assert
        _mockGameStateStorage.VerifyUndoAsyncCalled(Alias, GameId, Times.Once);
    }

    [Fact]
    public async Task UndoAsync_WhenOnInitialGameState_ShouldNotUndoGameState()
    {
        // Arrange
        var gameState = Container
            .Build<GameStateMemory>()
            .With(x => x.TotalMoves, 1)
            .Create();
        _mockLocalStorageService.SetupLoadGameAsync(gameState);
        var sut = ResolveSut();

        // Act
        await sut.UndoGameAsync(Alias, GameId);

        // Assert
        _mockGameStateStorage.VerifyUndoAsyncCalled(Alias, GameId, Times.Never);
    }
}