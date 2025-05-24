using Sudoku.Web.Server.Services;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockLocalStorageServiceExtensions
{
    public static Mock<ILocalStorageService> SetupLoadGameAsync(this Mock<ILocalStorageService> mock, GameStateMemory? gameState)
    {
        mock
            .Setup(x => x.LoadGameAsync(It.IsAny<string>()))
            .ReturnsAsync(gameState);

        return mock;
    }

    public static Mock<ILocalStorageService> VerifyDeleteGameAsyncCalled(this Mock<ILocalStorageService> mock, Func<Times> times)
    {
        mock.Verify(x => x.DeleteGameAsync(It.IsAny<string>()), times);

        return mock;
    }

    public static Mock<ILocalStorageService> VerifySaveGameAsyncCalled(this Mock<ILocalStorageService> mock, Func<Times> times)
    {
        mock.Verify(x => x.SaveGameStateAsync(It.IsAny<GameStateMemory>()), Times.Once);

        return mock;
    }

    public static Mock<ILocalStorageService> VerifySaveAsyncCalledWith(this Mock<ILocalStorageService> mock, GameStateMemory gameState, Func<Times> times)
    {
        mock.Verify(x => x.SaveGameStateAsync(gameState), times);

        return mock;
    }

    public static Mock<ILocalStorageService> VerifyLoadGameAsyncCalled(this Mock<ILocalStorageService> mock, Func<Times> times)
    {
        mock.Verify(x => x.LoadGameAsync(It.IsAny<string>()), times);

        return mock;
    }

    public static Mock<ILocalStorageService> VerifyLoadGamesAsyncCalled(this Mock<ILocalStorageService> mock, Func<Times> times)
    {
        mock.Verify(x => x.LoadGameStatesAsync(), times);
        return mock;
    }
}