using Sudoku.Web.Server.Services;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockGameStateManagerExtensions
{
    public static Mock<IGameStateManager> SetupLoadGameAsync(this Mock<IGameStateManager> mock, GameStateMemory gameState)
    {
        mock
            .Setup(x => x.LoadGameAsync(It.IsAny<string>()))
            .ReturnsAsync(gameState);

        return mock;
    }

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

    public static Mock<IGameStateManager> VerifyDeleteGameAsyncCalled(this Mock<IGameStateManager> mock, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.DeleteGameAsync(puzzleId), times);

        return mock;
    }

    public static Mock<IGameStateManager> VerifyLoadsAsyncCalled(this Mock<IGameStateManager> mock, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.LoadGameAsync(puzzleId), times);

        return mock;
    }

    public static Mock<IGameStateManager> VerifyResetAsyncCalled(this Mock<IGameStateManager> mock, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.ResetGameAsync(puzzleId), times);

        return mock;
    }

    public static Mock<IGameStateManager> VerifySaveAsyncCalled(this Mock<IGameStateManager> mock, Func<Times> times)
    {
        mock.Verify(x => x.SaveGameAsync(It.IsAny<GameStateMemory>()), times);

        return mock;
    }

    public static Mock<IGameStateManager> VerifyUndoAsyncCalled(this Mock<IGameStateManager> mock, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.UndoGameAsync(puzzleId), times);

        return mock;
    }
}