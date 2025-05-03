using Sudoku.Web.Server.Services;
using XenobiaSoft.Sudoku;

namespace UnitTests.Helpers.Mocks;

public static class MockInvalidCellNotificationService
{
    public static Mock<IInvalidCellNotificationService> VerifyNotificationSent(this Mock<IInvalidCellNotificationService> mock, Func<Times> times)
    {
        mock.Verify(x => x.Notify(It.IsAny<IEnumerable<Cell>>()), times);

        return mock;
    }
}