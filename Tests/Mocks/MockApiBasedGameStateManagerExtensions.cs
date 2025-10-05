using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.Abstractions.V2;

namespace UnitTests.Mocks;

public static class MockApiBasedGameStateManagerExtensions
{
    public static void SetupCreateGameAsync(this Mock<IGameStateManager> mock, string alias, string difficulty)
    {
        var game = new GameModel
        {
            Id = Guid.NewGuid().ToString(),
            PlayerAlias = alias,
            Difficulty = difficulty,
            Status = "New",
            Statistics = new(),
            CreatedAt = DateTime.UtcNow,
            StartedAt = null,
            CompletedAt = null,
            PausedAt = null,
            Cells = []
        };
        mock
            .Setup(x => x.CreateGameAsync(alias, difficulty))
            .ReturnsAsync(game);
    }

    public static void SetupLoadGameAsync(this Mock<IGameStateManager> mock, GameModel game)
    {
        mock
            .Setup(x => x.LoadGameAsync(game.Alias, It.IsAny<string>()))
            .ReturnsAsync(game);
    }

    public static void SetupLoadGamesAsync(this Mock<IGameStateManager> mock, IEnumerable<GameModel> gameStates)
    {
        mock
            .Setup(x => x.LoadGamesAsync(It.IsAny<string>()))
            .ReturnsAsync(gameStates.ToList());
    }

    public static void VerifyCreateGameAsync(this Mock<IGameStateManager> mock, string alias, string difficulty, Func<Times> times)
    {
        mock.Verify(x => x.CreateGameAsync(alias, difficulty), times);
    }

    public static void VerifyDeleteGameAsyncCalled(this Mock<IGameStateManager> mock, Func<Times> times)
    {
        mock.Verify(x => x.DeleteGameAsync(It.IsAny<string>(), It.IsAny<string>()), times);
    }

    public static void VerifyDeleteGameAsyncCalled(this Mock<IGameStateManager> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.DeleteGameAsync(alias, puzzleId), times);
    }

    public static void VerifyLoadsAsyncCalled(this Mock<IGameStateManager> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.LoadGameAsync(alias, puzzleId), times);
    }

    public static void VerifyResetAsyncCalled(this Mock<IGameStateManager> mock, Func<Times> times)
    {
        mock.Verify(x => x.ResetGameAsync(), times);
    }

    public static void VerifySaveAsyncCalled(this Mock<IGameStateManager> mock, Func<Times> times)
    {
        mock.Verify(x => x.SaveGameAsync(), times);
    }

    public static void VerifyUndoAsyncCalled(this Mock<IGameStateManager> mock, Func<Times> times)
    {
        mock.Verify(x => x.UndoGameAsync(), times);
    }
}