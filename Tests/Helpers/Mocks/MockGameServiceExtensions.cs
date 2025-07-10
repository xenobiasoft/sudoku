using XenobiaSoft.Sudoku.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockGameServiceExtensions
{
    public static void SetUpReturnedGames(this Mock<IGameService> mock, IEnumerable<GameStateMemory> games)
    {
        mock.Setup(x => x.LoadGamesAsync(It.IsAny<string>())).ReturnsAsync(games);
    }

    public static void VerifyGetGamesForAliasCalled(this Mock<IGameService> mock, string alias, Func<Times> times)
    {
        mock.Verify(x => x.LoadGamesAsync(alias), times);
    }
}