using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockGameServiceExtensions
{
    public static void SetUpReturnedGame(this Mock<IGameService> mock, GameStateMemory game)
    {
        mock.Setup(x => x.LoadGameAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(game);
    }

    public static void SetUpReturnedGames(this Mock<IGameService> mock, IEnumerable<GameStateMemory> games)
    {
        mock.Setup(x => x.LoadGamesAsync(It.IsAny<string>())).ReturnsAsync(games);
    }

    public static void VerifyCreateGameAsyncCalled(this Mock<IGameService> mock, string alias, GameDifficulty difficulty, Func<Times> times)
    {
        mock.Verify(x => x.CreateGameAsync(alias, difficulty), times);
    }

    public static void VerifyDeleteGameAsyncCalled(this Mock<IGameService> mock, string alias, string gameId, Func<Times> times)
    {
        mock.Verify(x => x.DeleteGameAsync(alias, gameId), times);
    }

    public static void VerifyGetGamesForAliasCalled(this Mock<IGameService> mock, string alias, Func<Times> times)
    {
        mock.Verify(x => x.LoadGamesAsync(alias), times);
    }

    public static void VerifyGetGameForAliasCalled(this Mock<IGameService> mock, string alias, string gameId, Func<Times> times)
    {
        mock.Verify(x => x.LoadGameAsync(alias, gameId), times);
    }
}