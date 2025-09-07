using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Mocks;

public static class MockGameManagerExtensions
{
    public static void SetupLoadGameAsync(this Mock<IGameManager> mock, GameStateMemory gameState)
    {
        mock
            .Setup(x => x.LoadGameAsync(gameState.Alias, It.IsAny<string>()))
            .ReturnsAsync(gameState);
    }

    public static void SetupLoadGamesAsync(this Mock<IGameManager> mock, IEnumerable<GameStateMemory> gameStates)
    {
        mock
            .Setup(x => x.LoadGamesAsync())
            .ReturnsAsync(gameStates.ToList());
    }

    public static void VerifyDeleteGameAsyncCalled(this Mock<IGameManager> mock, Func<Times> times)
    {
        mock.Verify(x => x.DeleteGameAsync(It.IsAny<string>(), It.IsAny<string>()), times);
    }

    public static void VerifyDeleteGameAsyncCalled(this Mock<IGameManager> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.DeleteGameAsync(alias, puzzleId), times);
    }

    public static void VerifyLoadsAsyncCalled(this Mock<IGameManager> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.LoadGameAsync(alias, puzzleId), times);
    }

    public static void VerifyMoveRecorded(this Mock<IGameManager> mock, Func<Times> times)
    {
        mock.Verify(x => x.RecordMove(It.IsAny<bool>()), times);
    }

    public static void VerifyResetAsyncCalled(this Mock<IGameManager> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.ResetGameAsync(alias, puzzleId), times);
    }

    public static void VerifySaveAsyncCalled(this Mock<IGameManager> mock, Func<Times> times)
    {
        mock.Verify(x => x.SaveGameAsync(It.IsAny<GameStateMemory>()), times);
    }

    public static void VerifyUndoAsyncCalled(this Mock<IGameManager> mock, string alias, string puzzleId, Func<Times> times)
    {
        mock.Verify(x => x.UndoGameAsync(alias, puzzleId), times);
    }
}