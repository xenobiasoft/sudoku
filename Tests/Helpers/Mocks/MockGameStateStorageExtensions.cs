using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockGameStateStorageExtensions
{
    public static Mock<IGameStateStorage<TStateMemoryType>> SetupEmptyStack<TStateMemoryType>(this Mock<IGameStateStorage<TStateMemoryType>> mock) where TStateMemoryType : PuzzleState
    {
        mock
            .Setup(x => x.UndoAsync(It.IsAny<string>()))
            .ReturnsAsync((TStateMemoryType?)null);

        return mock;
    }

    public static Mock<IGameStateStorage<TStateMemoryType>> VerifyDeleteAsyncCalled<TStateMemoryType>(this Mock<IGameStateStorage<TStateMemoryType>> mock, Func<Times> times) where TStateMemoryType : PuzzleState
    {
        mock.Verify(x => x.DeleteAsync(It.IsAny<string>()), times);

        return mock;
    }

    public static Mock<IGameStateStorage<TStateMemoryType>> VerifyLoadAsyncCalled<TStateMemoryType>(this Mock<IGameStateStorage<TStateMemoryType>> mock, Func<Times> times) where TStateMemoryType : PuzzleState
    {
        mock.Verify(x => x.LoadAsync(It.IsAny<string>()), times);

        return mock;
    }

    public static Mock<IGameStateStorage<TStateMemoryType>> VerifySaveAsyncCalled<TStateMemoryType>(this Mock<IGameStateStorage<TStateMemoryType>> mock, Func<Times> times) where TStateMemoryType : PuzzleState
    {
        mock.Verify(x => x.SaveAsync(It.IsAny<TStateMemoryType>()), times);

        return mock;
    }

    public static Mock<IGameStateStorage<TStateMemoryType>> VerifyUndoAsyncCalled<TStateMemoryType>(this Mock<IGameStateStorage<TStateMemoryType>> mock, Func<Times> times) where TStateMemoryType : PuzzleState
    {
        mock.Verify(x => x.UndoAsync(It.IsAny<string>()), times);

        return mock;
    }
}