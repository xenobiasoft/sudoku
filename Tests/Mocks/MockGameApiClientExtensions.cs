using Sudoku.Web.Server.Services.HttpClients;

namespace UnitTests.Mocks;

public static class MockGameApiClientExtensions
{
    public static void VerifyMakesMove(this Mock<IGameApiClient> mock, string alias, string gameId, int row, int column, int? value)
    {
        mock.Verify(x => x.MakeMoveAsync(alias, gameId, row, column, value), Times.Once);
    }
}