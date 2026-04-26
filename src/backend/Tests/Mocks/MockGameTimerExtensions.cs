using Sudoku.Blazor.Services.Abstractions;

namespace UnitTests.Mocks;

public static class MockGameTimerExtensions
{
    extension(Mock<IGameTimer> mock)
    {
        public Mock<IGameTimer> RaiseTick()
        {
            mock.Raise(x => x.OnTick += null, new object(), TimeSpan.FromSeconds(1));

            return mock;
        }

        public Mock<IGameTimer> VerifyPaused(Func<Times> times)
        {
            mock.Verify(t => t.Pause(), times);
        
            return mock;
        }

        public Mock<IGameTimer> VerifyReset(Func<Times> times)
        {
            mock.Verify(t => t.Reset(), times);
        
            return mock;
        }

        public Mock<IGameTimer> VerifyResumed(Func<Times> times)
        {
            mock.Verify(t => t.Resume(It.IsAny<TimeSpan>()), times);
        
            return mock;
        }

        public Mock<IGameTimer> VerifyStarted(Func<Times> times)
        {
            mock.Verify(t => t.Start(), times);
        
            return mock;
        }
    }
}