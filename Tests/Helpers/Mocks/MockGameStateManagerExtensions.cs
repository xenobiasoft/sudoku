using Sudoku.Web.Server.Services;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockGameStateManagerExtensions
{
    public static Mock<IGameStateManager> SetupLoadGamesAsync(this Mock<IGameStateManager> mock, IEnumerable<GameStateMemory> gameStates)
    {
        mock
            .Setup(x => x.LoadGamesAsync())
            .ReturnsAsync(gameStates.ToList);

        return mock;
    }

    public static Mock<IGameStateManager> VerifyDeleteGameAsyncCalled(this Mock<IGameStateManager> mock, Func<Times> times)
    {
        mock.Verify(x => x.DeleteGameAsync(It.IsAny<string>()), times);

        return mock;
    }

    public static Mock<IGameStateManager> VerifySaveAsyncCalled(this Mock<IGameStateManager> mock, Func<Times> times)
    {
        mock.Verify(x => x.SaveGameAsync(It.IsAny<GameStateMemory>()), times);

        return mock;
    }
}