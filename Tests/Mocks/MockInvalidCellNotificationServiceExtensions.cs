using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku;

namespace UnitTests.Mocks;

public static class MockInvalidCellNotificationServiceExtensions
{
    public static Mock<IInvalidCellNotificationService> VerifyNotificationSent(this Mock<IInvalidCellNotificationService> mock, Func<Times> times)
    {
        mock.Verify(x => x.Notify(It.IsAny<IEnumerable<Cell>>()), times);

        return mock;
    }
}