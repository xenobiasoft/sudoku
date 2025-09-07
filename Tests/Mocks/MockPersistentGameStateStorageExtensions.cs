using XenobiaSoft.Sudoku.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Mocks;

public static class MockPersistentGameStateStorageExtensions
{
    public static void VerifyDeleteGameAsyncCalled(this Mock<IPersistentGameStateStorage> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.DeleteAsync(alias, puzzleId), times);
    }

    public static void VerifyLoadGameAsyncCalled(this Mock<IPersistentGameStateStorage> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.LoadAsync(alias, puzzleId), times);
    }

    public static void VerifyLoadAllAsyncCalled(this Mock<IPersistentGameStateStorage> mock, string alias, Func<Times> times)
    {
        mock.Verify(x => x.LoadAllAsync(alias), times);
    }

    public static void VerifyResetGameAsyncCalled(this Mock<IPersistentGameStateStorage> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.ResetAsync(alias, puzzleId), times);
    }

    public static void VerifySaveGameAsyncCalled(this Mock<IPersistentGameStateStorage> mock, GameStateMemory gameState, Func<Times> times)
    {
        mock.Verify(x => x.SaveAsync(gameState), times);
    }

    public static void VerifyUndoAsyncCalled(this Mock<IPersistentGameStateStorage> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.UndoAsync(alias, puzzleId), times);
    }
}