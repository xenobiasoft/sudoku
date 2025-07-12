using XenobiaSoft.Sudoku.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockGameStateStorageExtensions
{
    public static Mock<T> SetupLoadAsync<T>(this Mock<T> mock, GameStateMemory gameState) where T : class, IGameStateStorage
    {
        mock
            .Setup(x => x.LoadAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(gameState);

        return mock;
    }

    public static Mock<T> SetupResetAsync<T>(this Mock<T> mock, GameStateMemory? gameState) where T : class, IGameStateStorage
    {
        mock
            .Setup(x => x.ResetAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(gameState)
            .Verifiable();

        return mock;
    }

    public static Mock<T> SetupUndoAsync<T>(this Mock<T> mock, GameStateMemory? gameState) where T : class, IGameStateStorage
    {
        mock
            .Setup(x => x.UndoAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(gameState)
            .Verifiable();

        return mock;
    }

    public static Mock<T> VerifyDeleteAsyncCalled<T>(this Mock<T> mock, Func<Times> times) where T : class, IGameStateStorage
    {
        mock.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<string>()), times);

        return mock;
    }

    public static Mock<T> VerifyDeleteAsyncCalled<T>(this Mock<T> mock, string alias, string puzzleId, Func<Times> times) where T : class, IGameStateStorage
    {
        mock.Verify(x => x.DeleteAsync(alias, puzzleId), times);

        return mock;
    }

    public static Mock<T> VerifyLoadAsyncCalled<T>(this Mock<T> mock, string alias, string puzzleId, Func<Times> times) where T : class, IGameStateStorage
    {
        mock.Verify(x => x.LoadAsync(alias, puzzleId), times);

        return mock;
    }

    public static Mock<T> VerifyResetAsyncCalled<T>(this Mock<T> mock, string alias, string puzzleId, Func<Times> times) where T : class, IGameStateStorage
    {
        mock.Verify(x => x.ResetAsync(alias, puzzleId), times);

        return mock;
    }

    public static Mock<T> VerifySaveAsyncCalled<T>(this Mock<T> mock, Func<Times> times) where T : class, IGameStateStorage
    {
        mock.Verify(x => x.SaveAsync(It.IsAny<GameStateMemory>()), times);

        return mock;
    }

    public static Mock<T> VerifySaveAsyncCalled<T>(this Mock<T> mock, GameStateMemory gameState, Func<Times> times) where T : class, IGameStateStorage
    {
        mock.Verify(x => x.SaveAsync(gameState), times);

        return mock;
    }

    public static Mock<T> VerifyUndoAsyncCalled<T>(this Mock<T> mock, string alias, string puzzleId, Func<Times> times) where T : class, IGameStateStorage
    {
        mock.Verify(x => x.UndoAsync(alias, puzzleId), times);

        return mock;
    }
}