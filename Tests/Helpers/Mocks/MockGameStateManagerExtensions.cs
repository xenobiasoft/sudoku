using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockGameStateManagerExtensions
{
    public static Mock<IGameStateManager> SetupGetAliasAsync(this Mock<IGameStateManager> mock, string alias)
    {
        mock
            .Setup(x => x.GetGameAliasAsync())
            .ReturnsAsync(alias);

        return mock;
    }

    public static Mock<IGameStateManager> SetupLoadGameAsync(this Mock<IGameStateManager> mock, GameStateMemory gameState)
    {
        mock
            .Setup(x => x.LoadGameAsync(gameState.Alias, It.IsAny<string>()))
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
        mock.Verify(x => x.DeleteGameAsync(It.IsAny<string>(), It.IsAny<string>()), times);

        return mock;
    }

    public static Mock<IGameStateManager> VerifyDeleteGameAsyncCalled(this Mock<IGameStateManager> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.DeleteGameAsync(alias, puzzleId), times);

        return mock;
    }

    public static Mock<IGameStateManager> VerifyLoadsAsyncCalled(this Mock<IGameStateManager> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.LoadGameAsync(alias, puzzleId), times);

        return mock;
    }

    public static Mock<IGameStateManager> VerifyResetAsyncCalled(this Mock<IGameStateManager> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.ResetGameAsync(alias, puzzleId), times);

        return mock;
    }

    public static Mock<IGameStateManager> VerifySaveAsyncCalled(this Mock<IGameStateManager> mock, Func<Times> times)
    {
        mock.Verify(x => x.SaveGameAsync(It.IsAny<GameStateMemory>()), times);

        return mock;
    }

    public static Mock<IGameStateManager> VerifyUndoAsyncCalled(this Mock<IGameStateManager> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.UndoGameAsync(alias, puzzleId), times);

        return mock;
    }
}