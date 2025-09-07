using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku;

namespace UnitTests.Mocks;

public static class MockNotificationServiceExtensions
{
    public static Mock<INotificationService> VerifyGameStartedSent(this Mock<INotificationService> mock, Func<Times> times)
    {
        mock.Verify(x => x.NotifyGameStarted(), times);
        return mock;
    }

    public static Mock<INotificationService> VerifyGameEndedSent(this Mock<INotificationService> mock, Func<Times> times)
    {
        mock.Verify(x => x.NotifyGameEnded(), times);
        return mock;
    }

    public static Mock<INotificationService> VerifyInvalidCellsNotified(this Mock<INotificationService> mock, Func<Times> times)
    {
        mock.Verify(x => x.NotifyInvalidCells(It.IsAny<IEnumerable<Cell>>()), times);
        return mock;
    }

    public static Mock<INotificationService> VerifyCellFocusNotified(this Mock<INotificationService> mock, Func<Times> times)
    {
        mock.Verify(x => x.NotifyCellFocused(It.IsAny<Cell>()), times);
        return mock;
    }
}