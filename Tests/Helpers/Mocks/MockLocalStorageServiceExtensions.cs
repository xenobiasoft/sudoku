using Moq;
using Sudoku.Web.Server.Services;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockLocalStorageServiceExtensions
{
    public static Mock<ILocalStorageService> SetupLoadGameAsync(this Mock<ILocalStorageService> mock, string gameId, GameStateMemory? gameState)
    {
        mock
            .Setup(x => x.LoadGameAsync(gameId))
            .ReturnsAsync(gameState);

        return mock;
    }

    public static Mock<ILocalStorageService> SetupLoadSavedGames(this Mock<ILocalStorageService> mock, List<GameStateMemory> savedGames)
    {
        mock
            .Setup(x => x.LoadGameStatesAsync())
            .ReturnsAsync(savedGames);

        return mock;
    }

    public static Mock<ILocalStorageService> VerifyDeleteGameAsyncCalled(this Mock<ILocalStorageService> mock,
        Func<Times> times)
    {
        mock.Verify(x => x.DeleteGameAsync(It.IsAny<string>()), times);

        return mock;
    }

    public static Mock<ILocalStorageService> VerifySaveGameAsyncCalled(this Mock<ILocalStorageService> mock,
        Func<Times> times)
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
}