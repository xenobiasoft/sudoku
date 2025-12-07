using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.Abstractions.V2;

namespace UnitTests.Mocks;

public static class MockLocalStorageServiceV2Extensions
{
    public static void SetupGetAliasAsync(this Mock<ILocalStorageService> mock, string? alias)
    {
        mock
            .Setup(x => x.GetAliasAsync())
            .ReturnsAsync(alias);
    }

    public static void SetupLoadGameAsync(this Mock<ILocalStorageService> mock, GameModel? gameState)
    {
        mock
            .Setup(x => x.LoadGameAsync(It.IsAny<string>()))
            .ReturnsAsync(gameState);
    }

    public static void VerifyDeleteGameAsyncCalled(this Mock<ILocalStorageService> mock, Func<Times> times)
    {
        mock.Verify(x => x.DeleteGameAsync(It.IsAny<string>()), times);
    }

    public static void VerifySaveGameAsyncCalled(this Mock<ILocalStorageService> mock, Func<Times> times)
    {
        mock.Verify(x => x.SaveGameStateAsync(It.IsAny<GameModel>()), Times.Once);
    }

    public static void VerifySetAliasAsyncCalled(this Mock<ILocalStorageService> mock, Func<Times> times)
    {
        mock.Verify(x => x.SetAliasAsync(It.IsAny<string>()), times);
    }

    public static void VerifyLoadGameAsyncCalled(this Mock<ILocalStorageService> mock, Func<Times> times)
    {
        mock.Verify(x => x.LoadGameAsync(It.IsAny<string>()), times);
    }

    public static void VerifyLoadGamesAsyncCalled(this Mock<ILocalStorageService> mock, Func<Times> times)
    {
        mock.Verify(x => x.LoadGameStatesAsync(), times);
    }
}