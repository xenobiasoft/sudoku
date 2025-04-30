using Sudoku.Web.Server.Services;

namespace UnitTests.Helpers.Mocks;

public static class MockGameNotificationService
{
    public static Mock<IGameNotificationService> VerifyGameEndedSent(this Mock<IGameNotificationService> mock, Func<Times> times)
    {
        mock.Verify(x => x.NotifyGameEnded(), times);

        return mock;
    }

    public static Mock<IGameNotificationService> VerifyGameStartedSent(this Mock<IGameNotificationService> mock, Func<Times> times)
    {
        mock.Verify(x => x.NotifyGameStarted(), times);

        return mock;
    }
}