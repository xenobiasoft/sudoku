using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Services;

namespace UnitTests.Helpers;

public static class MockStorageServiceExtensions
{
    public static void VerifyDeletesBlob(this Mock<IStorageService> mock, string containerName, string blobName, Func<Times> times)
    {
        mock.Verify(s => s.DeleteAsync(containerName, blobName), times);
    }

    public static void VerifyLoadsGameState(this Mock<IStorageService> mock, string containerName, string blobName, Func<Times> times)
    {
        mock.Verify(s => s.LoadAsync<GameStateMemento>(containerName, blobName), times);
    }

    public static void VerifySavesGameState(this Mock<IStorageService> mock, string containerName, string blobName, GameStateMemento expectedGameState, Func<Times> times)
    {
        mock.Verify(s => s.SaveAsync(containerName, blobName, expectedGameState, It.IsAny<CancellationToken>()), times);
    }
}