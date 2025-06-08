using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockGameStateStorageExtensions
{
    public static Mock<IGameStateStorage> SetupLoadAsync(this Mock<IGameStateStorage> mock, GameStateMemory gameState)
    {
        mock
            .Setup(x => x.LoadAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(gameState);

        return mock;
    }

    public static Mock<IGameStateStorage> SetupResetAsync(this Mock<IGameStateStorage> mock, GameStateMemory? gameState)
    {
        mock
            .Setup(x => x.ResetAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(gameState)
            .Verifiable();

        return mock;
    }

    public static Mock<IGameStateStorage> SetupUndoAsync(this Mock<IGameStateStorage> mock, GameStateMemory? gameState)
    {
        mock
            .Setup(x => x.UndoAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(gameState)
            .Verifiable();

        return mock;
    }

    public static Mock<IGameStateStorage> VerifyDeleteAsyncCalled(this Mock<IGameStateStorage> mock, Func<Times> times)
    {
        mock.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<string>()), times);

        return mock;
    }

    public static Mock<IGameStateStorage> VerifyDeleteAsyncCalled(this Mock<IGameStateStorage> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.DeleteAsync(alias, puzzleId), times);

        return mock;
    }

    public static Mock<IGameStateStorage> VerifyLoadAsyncCalled(this Mock<IGameStateStorage> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.LoadAsync(alias, puzzleId), times);

        return mock;
    }

    public static Mock<IGameStateStorage> VerifyResetAsyncCalled(this Mock<IGameStateStorage> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.ResetAsync(alias, puzzleId), times);

        return mock;
    }

    public static Mock<IGameStateStorage> VerifySaveAsyncCalled(this Mock<IGameStateStorage> mock, Func<Times> times)
    {
        mock.Verify(x => x.SaveAsync(It.IsAny<GameStateMemory>()), times);

        return mock;
    }

    public static Mock<IGameStateStorage> VerifySaveAsyncCalled(this Mock<IGameStateStorage> mock, GameStateMemory gameState, Func<Times> times)
    {
        mock.Verify(x => x.SaveAsync(gameState), times);

        return mock;
    }

    public static Mock<IGameStateStorage> VerifyUndoAsyncCalled(this Mock<IGameStateStorage> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.UndoAsync(alias, puzzleId), times);

        return mock;
    }
}