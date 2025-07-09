using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Services;

namespace UnitTests.Helpers.Mocks;

public static class MockGameServiceExtensions
{
    public static void SetUpReturnedGames(this Mock<IGameService> mock, IEnumerable<GameStateMemory> games)
    {
        mock.Setup(x => x.GetGamesForPlayerAsync(It.IsAny<string>())).ReturnsAsync(games);
    }

    public static void VerifyGetGamesForAliasCalled(this Mock<IGameService> mock, string alias, Func<Times> times)
    {
        mock.Verify(x => x.GetGamesForPlayerAsync(alias), times);
    }
}