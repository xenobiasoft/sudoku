using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.Abstractions;

namespace UnitTests.Mocks;

public static class MockLocalStorageServiceV2Extensions
{
    extension(Mock<ILocalStorageService> mock)
    {
        public void SetupGetAliasAsync(string? alias)
        {
            mock
                .Setup(x => x.GetAliasAsync())
                .ReturnsAsync(alias);
        }

        public void SetupLoadGameAsync(GameModel? gameState)
        {
            mock
                .Setup(x => x.LoadGameAsync(It.IsAny<string>()))
                .ReturnsAsync(gameState);
        }

        public void VerifyDeleteGameAsyncCalled(Func<Times> times)
        {
            mock.Verify(x => x.DeleteGameAsync(It.IsAny<string>()), times);
        }

        public void VerifySaveGameAsyncCalled(Func<Times> times)
        {
            mock.Verify(x => x.SaveGameStateAsync(It.IsAny<GameModel>()), Times.Once);
        }

        public void VerifySetAliasAsyncCalled(Func<Times> times)
        {
            mock.Verify(x => x.SetAliasAsync(It.IsAny<string>()), times);
        }

        public void VerifyLoadGameAsyncCalled(Func<Times> times)
        {
            mock.Verify(x => x.LoadGameAsync(It.IsAny<string>()), times);
        }

        public void VerifyLoadGamesAsyncCalled(Func<Times> times)
        {
            mock.Verify(x => x.LoadGameStatesAsync(), times);
        }
    }
}