using Sudoku.Web.Server.Services.Abstractions;

namespace UnitTests.Helpers.Mocks;

public static class MockGameTimerExtensions
{
    public static Mock<IGameTimer> RaiseTick(this Mock<IGameTimer> mock)
    {
        mock.Raise(x => x.OnTick += null, new object(), TimeSpan.FromSeconds(1));

        return mock;
    }

    public static Mock<IGameTimer> VerifyPaused(this Mock<IGameTimer> mock, Func<Times> times)
    {
        mock.Verify(t => t.Pause(), times);
        
        return mock;
    }

    public static Mock<IGameTimer> VerifyReset(this Mock<IGameTimer> mock, Func<Times> times)
    {
        mock.Verify(t => t.Reset(), times);
        
        return mock;
    }

    public static Mock<IGameTimer> VerifyResumed(this Mock<IGameTimer> mock, Func<Times> times)
    {
        mock.Verify(t => t.Resume(It.IsAny<TimeSpan>()), times);
        
        return mock;
    }

    public static Mock<IGameTimer> VerifyStarted(this Mock<IGameTimer> mock, Func<Times> times)
    {
        mock.Verify(t => t.Start(), times);
        
        return mock;
    }
}