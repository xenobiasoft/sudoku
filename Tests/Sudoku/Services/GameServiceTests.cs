using DepenMock.XUnit;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku.Abstractions;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Services;

namespace UnitTests.Sudoku.Services;

public class GameServiceTests : BaseTestByAbstraction<GameService, IGameService>
{
    private readonly string _alias;
    private readonly string _gameId;
    private readonly IGameService _sut;
    private readonly Mock<IPersistentGameStateStorage> _mockPersistentGameStateStorage;

    public GameServiceTests()
    {
        _alias = Container.Create<string>();
        _gameId = Container.Create<string>();
        _mockPersistentGameStateStorage = Container.ResolveMock<IPersistentGameStateStorage>();
        _sut = ResolveSut();
    }

    [Fact]
    public async Task DeleteGameAsync_CallsStorageService_DeleteGameAsync()
    {
        // Act
        await _sut.DeleteGameAsync(_alias, _gameId);

        // Assert
        _mockPersistentGameStateStorage.VerifyDeleteGameAsyncCalled(_alias, _gameId, Times.Once);
    }

    [Fact]
    public async Task LoadGameAsync_CallsStorageService_LoadGame()
    {
        // Act
        await _sut.LoadGameAsync(_alias, _gameId);

        // Assert
        _mockPersistentGameStateStorage.VerifyLoadGameAsyncCalled(_alias, _gameId, Times.Once);
    }

	[Fact]
	public async Task LoadGamesAsync_CallsStorageService_LoadGames()
	{
		// Act
        await _sut.LoadGamesAsync(_alias);

		// Assert
        _mockPersistentGameStateStorage.VerifyLoadAllAsyncCalled(_alias, Times.Once);
    }

    [Fact]
    public async Task ResetGameAsync_CallsStorageService_ResetAsync()
    {
        // Act
        await _sut.ResetGameAsync(_alias, _gameId);

        // Assert
        _mockPersistentGameStateStorage.VerifyResetGameAsyncCalled(_alias, _gameId, Times.Once);
    }

    [Fact]
    public async Task SaveGameAsync_CallsStorageService_SaveGameAsync()
    {
        // Arrange
        var gameState = Container.Create<GameStateMemory>();

        // Act
        await _sut.SaveGameAsync(gameState);

        // Assert
        _mockPersistentGameStateStorage.VerifySaveGameAsyncCalled(gameState, Times.Once);
    }

    [Fact]
    public async Task UndoGameAsync_CallsStorageService_UndoGameAsync()
    {
        // Act
        await _sut.UndoGameAsync(_alias, _gameId);

        // Assert
        _mockPersistentGameStateStorage.VerifyUndoAsyncCalled(_alias, _gameId, Times.Once);
    }
}