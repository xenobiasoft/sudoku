using XenobiaSoft.Sudoku.Abstractions;

namespace UnitTests.Mocks;

public static class MockPlayerServiceExtensions
{
    public static Mock<IPlayerService> VerifyCreateNewCalled(this Mock<IPlayerService> mock, Func<Times> times)
    {
        mock.Verify(x => x.CreateNewAsync(), times);

        return mock;
    }
}