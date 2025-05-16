using Sudoku.Web.Server.Services;

namespace UnitTests.Helpers;

public static class GameSessionExtensions
{
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