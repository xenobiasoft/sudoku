using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers;

public static class GameSessionExtensions
{
    public static IGameSession VerifyGameSessionReloaded(this IGameSession gameSession, GameStateMemory gameState)
    {
        gameSession.GameState.Board.Should().BeEquivalentTo(gameState.Board);
        
        return gameSession;
    }

    public static IGameSession VerifyNewSession(this IGameSession gameSession, string puzzleId)
    {
        Assert.Multiple(() =>
        {
            gameSession.Should().BeOfType<GameSession>();
            gameSession.GameState.PuzzleId.Should().Be(puzzleId);
        });
        
        return gameSession;
    }

    public static IGameSession VerifyRecordMoveCalled(this IGameSession gameSession, int expectedInvalidMoves, int expectedTotalMoves)
    {
        gameSession.Should().BeOfType<GameSession>();
        gameSession.GameState.TotalMoves.Should().Be(expectedTotalMoves);
        gameSession.GameState.InvalidMoves.Should().Be(expectedInvalidMoves);
        
        return gameSession;
    }

    public static IGameSession VerifySessionReset(this IGameSession gameSession)
    {
        gameSession.Should().BeOfType<NullGameSession>();
        
        return gameSession;
    }
}