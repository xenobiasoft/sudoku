using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockGameStateMemoryExtensions
{
    public static Mock<IGameStateMemory> VerifyClearAsyncCalled(this Mock<IGameStateMemory> mock, Func<Times> times)
    {
        mock.Verify(x => x.ClearAsync(It.IsAny<string>()), times);

        return mock;
    }

    public static Mock<IGameStateMemory> VerifySaveAsyncCalled(this Mock<IGameStateMemory> mock, Func<Times> times)
    {
        mock.Verify(x => x.SaveAsync(It.IsAny<GameStateMemento>()), times);

        return mock;
    }

    public static Mock<IGameStateMemory> VerifyUndoAsyncCalled(this Mock<IGameStateMemory> mock, Func<Times> times)
    {
        mock.Verify(x => x.UndoAsync(It.IsAny<string>()), times);

        return mock;
    }
}