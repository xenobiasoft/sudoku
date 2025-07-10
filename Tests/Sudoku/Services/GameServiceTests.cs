using DepenMock.XUnit;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku.Abstractions;
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
}