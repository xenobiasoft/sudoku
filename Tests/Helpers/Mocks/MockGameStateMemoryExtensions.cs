using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockGameStateMemoryExtensions
{
    public static Mock<IGameStateManager> VerifyDeleteAsyncCalled(this Mock<IGameStateManager> mock, Func<Times> times)
    {
        mock.Verify(x => x.DeleteAsync(It.IsAny<string>()), times);

        return mock;
    }

    public static Mock<IGameStateManager> VerifySaveAsyncCalled(this Mock<IGameStateManager> mock, Func<Times> times)
    {
        mock.Verify(x => x.SaveAsync(It.IsAny<GameStateMemento>()), times);

        return mock;
    }

    public static Mock<IGameStateManager> VerifyUndoAsyncCalled(this Mock<IGameStateManager> mock, Func<Times> times)
    {
        mock.Verify(x => x.UndoAsync(It.IsAny<string>()), times);

        return mock;
    }
}