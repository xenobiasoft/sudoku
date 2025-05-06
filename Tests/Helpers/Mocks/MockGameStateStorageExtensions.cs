using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockGameStateStorageExtensions
{
    public static Mock<IGameStateStorage> SetupEmptyStack(this Mock<IGameStateStorage> mock)
    {
        mock
            .Setup(x => x.UndoAsync(It.IsAny<string>()))
            .ReturnsAsync((GameStateMemory?)null);

        return mock;
    }

    public static Mock<IGameStateStorage> VerifyDeleteAsyncCalled(this Mock<IGameStateStorage> mock, Func<Times> times)
    {
        mock.Verify(x => x.DeleteAsync(It.IsAny<string>()), times);

        return mock;
    }

    public static Mock<IGameStateStorage> VerifyLoadAsyncCalled(this Mock<IGameStateStorage> mock, Func<Times> times)
    {
        mock.Verify(x => x.LoadAsync(It.IsAny<string>()), times);

        return mock;
    }

    public static Mock<IGameStateStorage> VerifySaveAsyncCalled(this Mock<IGameStateStorage> mock, Func<Times> times)
    {
        mock.Verify(x => x.SaveAsync(It.IsAny<GameStateMemory>()), times);

        return mock;
    }

    public static Mock<IGameStateStorage> VerifyUndoAsyncCalled(this Mock<IGameStateStorage> mock, Func<Times> times)
    {
        mock.Verify(x => x.UndoAsync(It.IsAny<string>()), times);

        return mock;
    }
}