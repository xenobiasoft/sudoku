using Moq;
using Sudoku.Web.Server.Services;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockLocalStorageServiceExtensions
{
    public static Mock<ILocalStorageService> SetupLoadSavedGames(this Mock<ILocalStorageService> mock, List<GameStateMemory> savedGames)
    {
        mock
            .Setup(x => x.LoadGameStatesAsync())
            .ReturnsAsync(savedGames);

        return mock;
    }

    public static Mock<ILocalStorageService> VerifySaveGameAsyncCalled(this Mock<ILocalStorageService> mock,
        Func<Times> times)
    {
        mock.Verify(x => x.AddGameStateAsync(It.IsAny<GameStateMemory>()), Times.Once);

        return mock;
    }
}