using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockGameStateStorageExtensions
{
    public static Mock<IGameStateStorage<TStateMemoryType>> SetupLoadAsync<TStateMemoryType>(this Mock<IGameStateStorage<TStateMemoryType>> mock, TStateMemoryType gameState) where TStateMemoryType : PuzzleState
    {
        mock
            .Setup(x => x.LoadAsync(It.IsAny<string>()))
            .ReturnsAsync(gameState);

        return mock;
    }

    public static Mock<IGameStateStorage<TStateMemoryType>> SetupResetAsync<TStateMemoryType>(this Mock<IGameStateStorage<TStateMemoryType>> mock, TStateMemoryType? gameState) where TStateMemoryType : PuzzleState
    {
        mock
            .Setup(x => x.ResetAsync(It.IsAny<string>()))
            .ReturnsAsync(gameState)
            .Verifiable();

        return mock;
    }

    public static Mock<IGameStateStorage<TStateMemoryType>> SetupUndoAsync<TStateMemoryType>(this Mock<IGameStateStorage<TStateMemoryType>> mock, TStateMemoryType? gameState) where TStateMemoryType : PuzzleState
    {
        mock
            .Setup(x => x.UndoAsync(It.IsAny<string>()))
            .ReturnsAsync(gameState)
            .Verifiable();

        return mock;
    }

    public static Mock<IGameStateStorage<TStateMemoryType>> VerifyDeleteAsyncCalled<TStateMemoryType>(this Mock<IGameStateStorage<TStateMemoryType>> mock, Func<Times> times) where TStateMemoryType : PuzzleState
    {
        mock.Verify(x => x.DeleteAsync(It.IsAny<string>()), times);

        return mock;
    }

    public static Mock<IGameStateStorage<TStateMemoryType>> VerifyDeleteAsyncCalled<TStateMemoryType>(this Mock<IGameStateStorage<TStateMemoryType>> mock, string puzzleId, Func<Times> times) where TStateMemoryType : PuzzleState
    {
        mock.Verify(x => x.DeleteAsync(puzzleId), times);

        return mock;
    }

    public static Mock<IGameStateStorage<TStateMemoryType>> VerifyLoadAsyncCalled<TStateMemoryType>(this Mock<IGameStateStorage<TStateMemoryType>> mock, string puzzleId, Func<Times> times) where TStateMemoryType : PuzzleState
    {
        mock.Verify(x => x.LoadAsync(puzzleId), times);

        return mock;
    }

    public static Mock<IGameStateStorage<TStateMemoryType>> VerifyResetAsyncCalled<TStateMemoryType>(this Mock<IGameStateStorage<TStateMemoryType>> mock, string puzzleId, Func<Times> times) where TStateMemoryType : PuzzleState
    {
        mock.Verify(x => x.ResetAsync(puzzleId), times);

        return mock;
    }

    public static Mock<IGameStateStorage<TStateMemoryType>> VerifySaveAsyncCalled<TStateMemoryType>(this Mock<IGameStateStorage<TStateMemoryType>> mock, Func<Times> times) where TStateMemoryType : PuzzleState
    {
        mock.Verify(x => x.SaveAsync(It.IsAny<TStateMemoryType>()), times);

        return mock;
    }

    public static Mock<IGameStateStorage<TStateMemoryType>> VerifySaveAsyncCalled<TStateMemoryType>(this Mock<IGameStateStorage<TStateMemoryType>> mock, TStateMemoryType gameState, Func<Times> times) where TStateMemoryType : PuzzleState
    {
        mock.Verify(x => x.SaveAsync(gameState), times);

        return mock;
    }

    public static Mock<IGameStateStorage<TStateMemoryType>> VerifyUndoAsyncCalled<TStateMemoryType>(this Mock<IGameStateStorage<TStateMemoryType>> mock, string puzzleId, Func<Times> times) where TStateMemoryType : PuzzleState
    {
        mock.Verify(x => x.UndoAsync(puzzleId), times);

        return mock;
    }
}