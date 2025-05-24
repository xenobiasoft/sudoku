using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers;

public static class GameSessionExtensions
{
    public static IGameSession VerifyGameSessionReloaded(this IGameSession gameSession, GameStateMemory gameState)
    {
        gameSession.Board.Should().BeEquivalentTo(gameState.Board);
        
        return gameSession;
    }

    public static IGameSession VerifyNewSession(this IGameSession gameSession, string sessionName)
    {
        Assert.Multiple(() =>
        {
            gameSession.Should().BeOfType<GameSession>();
            gameSession.PuzzleId.Should().Be(sessionName);
        });
        
        return gameSession;
    }
    
    public static IGameSession VerifySessionReset(this IGameSession gameSession)
    {
        gameSession.Should().BeOfType<NullGameSession>();
        
        return gameSession;
    }
}