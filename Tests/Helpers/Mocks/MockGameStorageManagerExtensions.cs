using Sudoku.Web.Server.Services;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockGameStorageManagerExtensions
{
    public static Mock<IGameStorageManager> SetupLoadGameAsync(this Mock<IGameStorageManager> mock, GameStateMemory gameState)
    {
        mock
            .Setup(x => x.LoadGameAsync(It.IsAny<string>()))
            .ReturnsAsync(gameState);

        return mock;
    }
}