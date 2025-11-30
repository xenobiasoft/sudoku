using Sudoku.Web.Server.Services.HttpClients;

namespace UnitTests.Mocks;

public static class MockGameApiClientExtensions
{
    public static void VerifyMakesMove(this Mock<IGameApiClient> mock, string alias, string gameId, int row, int column, int? value, Func<Times> times)
    {
        mock.Verify(x => x.MakeMoveAsync(alias, gameId, row, column, value, It.IsAny<TimeSpan>()), times);
    }

    public static void VerifySavesGameStatus(this Mock<IGameApiClient> mock, string alias, string gameId, string gameStatus, Func<Times> times)
    {
        mock.Verify(x => x.SaveGameStatusAsync(alias, gameId, gameStatus), times);
    }
}