using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockApiBasedGameStateManagerExtensions
{
    public static void SetupCreateGameAsync(this Mock<IApiBasedGameStateManager> mock, string alias)
    {
        var game = new GameModel(
            Id: Guid.NewGuid().ToString(),
            PlayerAlias: alias,
            Difficulty: "Easy",
            Status: "New",
            Statistics: new GameStatisticsModel(
                TotalMoves: 0,
                ValidMoves: 0,
                InvalidMoves: 0,
                PlayDuration: TimeSpan.Zero,
                AccuracyPercentage: 100.0),
            CreatedAt: DateTime.UtcNow,
            StartedAt: null,
            CompletedAt: null,
            PausedAt: null,
            Cells: new List<CellModel>());
        var apiResult = new ApiResult<GameModel>
        {
            Value = game,
            Error = string.Empty,
            IsSuccess = true
        };
        mock
            .Setup(x => x.CreateGameAsync(alias, It.IsAny<string>()))
            .ReturnsAsync(apiResult);
    }

    public static void SetupLoadGameAsync(this Mock<IApiBasedGameStateManager> mock, GameStateMemory gameState)
    {
        //mock
        //    .Setup(x => x.LoadGameAsync(gameState.Alias, It.IsAny<string>()))
        //    .ReturnsAsync(gameState);
        Assert.Fail("No load game method");
    }

    public static void SetupLoadGamesAsync(this Mock<IApiBasedGameStateManager> mock, IEnumerable<GameStateMemory> gameStates)
    {
        //mock
        //    .Setup(x => x.LoadGamesAsync(It.IsAny<string>()))
        //    .ReturnsAsync(gameStates.ToList);
        Assert.Fail("No load games method");
    }

    public static void VerifyCreateGameAsync(this Mock<IApiBasedGameStateManager> mock, string alias, Func<Times> times)
    {
        mock.Verify(x => x.CreateGameAsync(alias, It.IsAny<string>()), times);
    }

    public static void VerifyDeleteGameAsyncCalled(this Mock<IApiBasedGameStateManager> mock, Func<Times> times)
    {
        mock.Verify(x => x.DeleteGameAsync(It.IsAny<string>(), It.IsAny<string>()), times);
    }

    public static void VerifyDeleteGameAsyncCalled(this Mock<IApiBasedGameStateManager> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.DeleteGameAsync(alias, puzzleId), times);
    }

    public static void VerifyLoadsAsyncCalled(this Mock<IApiBasedGameStateManager> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.LoadGameAsync(alias, puzzleId), times);
    }

    public static void VerifyResetAsyncCalled(this Mock<IApiBasedGameStateManager> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.ResetGameAsync(alias, puzzleId), times);
    }

    public static void VerifySaveAsyncCalled(this Mock<IApiBasedGameStateManager> mock, Func<Times> times)
    {
        //mock.Verify(x => x.SaveGameAsync(It.IsAny<GameStateMemory>()), times);
        Assert.Fail("No save method");
    }

    public static void VerifyUndoAsyncCalled(this Mock<IApiBasedGameStateManager> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.UndoGameAsync(alias, puzzleId), times);
    }
}