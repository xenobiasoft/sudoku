using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services;
using Sudoku.Blazor.Services.HttpClients;

namespace UnitTests.Mocks;

public static class MockGameApiClientExtensions
{
    extension(Mock<IGameApiClient> mock)
    {
        public void SetupGetGame(GameModel game)
        {
            mock.Setup(x => x.GetGameAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(ApiResult<GameModel>.Success(game));
        }

        public void VerifyMakesMove(string alias, string gameId, int row, int column, int? value, Func<Times> times)
        {
            mock.Verify(x => x.MakeMoveAsync(alias, gameId, row, column, value, It.IsAny<TimeSpan>()), times);
        }

        public void VerifySavesGameStatus(string alias, string gameId, string gameStatus, Func<Times> times)
        {
            switch (gameStatus)
            {
                case GameStatus.InProgress: mock.Verify(x => x.ResumeGameAsync(alias, gameId), times); break;
                case GameStatus.Paused:     mock.Verify(x => x.PauseGameAsync(alias, gameId), times); break;
                case GameStatus.Completed:  mock.Verify(x => x.CompleteGameAsync(alias, gameId), times); break;
                case GameStatus.Abandoned:  mock.Verify(x => x.AbandonGameAsync(alias, gameId), times); break;
            }
        }
    }
}