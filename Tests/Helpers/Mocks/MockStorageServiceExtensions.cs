using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Services;

namespace UnitTests.Helpers.Mocks;

public static class MockStorageServiceExtensions
{
    public static Mock<IStorageService> StubGetBlobNamesCall(this Mock<IStorageService> mock, IEnumerable<string> blobNames)
    {
        mock
            .Setup(s => s.GetBlobNamesAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(blobNames.ToAsyncEnumerable());

        return mock;
    }

    public static Mock<IStorageService> StubLoadAsyncCall(this Mock<IStorageService> mock, GameStateMemory gameState)
    {
        mock
            .Setup(s => s.LoadAsync<GameStateMemory>(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(gameState);

        return mock;
    }

    public static Mock<IStorageService> VerifyDeletesBlob(this Mock<IStorageService> mock, string containerName, string blobName, Func<Times> times)
    {
        mock.Verify(s => s.DeleteAsync(containerName, blobName), times);

        return mock;
    }

    public static Mock<IStorageService> VerifyLoadsGameState(this Mock<IStorageService> mock, string containerName, string blobName, Func<Times> times)
    {
        mock.Verify(s => s.LoadAsync<GameStateMemory>(containerName, blobName), times);

        return mock;
    }

    public static Mock<IStorageService> VerifySavesGameState(this Mock<IStorageService> mock, string containerName, string blobName, GameStateMemory expectedGameState, Func<Times> times)
    {
        mock.Verify(s => s.SaveAsync(containerName, blobName, expectedGameState, false), times);

        return mock;
    }
}