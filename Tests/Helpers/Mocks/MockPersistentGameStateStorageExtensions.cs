using XenobiaSoft.Sudoku.Abstractions;

namespace UnitTests.Helpers.Mocks;

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
}