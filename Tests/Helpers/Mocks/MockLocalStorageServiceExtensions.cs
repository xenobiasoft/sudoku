using Sudoku.Web.Server.Services;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockLocalStorageServiceExtensions
{
    public static Mock<ILocalStorageService> VerifySaveGameAsyncCalled(this Mock<ILocalStorageService> mock,
        Func<Times> times)
    {
        mock.Verify(x => x.SaveGameStateAsync(It.IsAny<GameStateMemory>()), Times.Once);

        return mock;
    }
}