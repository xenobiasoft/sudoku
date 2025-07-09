using XenobiaSoft.Sudoku.Services;

namespace UnitTests.Helpers.Mocks;

public static class MockPlayerServiceExtensions
{
    public static Mock<IPlayerService> VerifyCreateNewCalled(this Mock<IPlayerService> mock, Func<Times> times)
    {
        mock.Verify(x => x.CreateNewAsync(), times);

        return mock;
    }
}