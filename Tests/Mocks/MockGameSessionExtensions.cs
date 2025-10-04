using Sudoku.Web.Server.Services.Abstractions;

namespace UnitTests.Mocks;

public static class MockGameSessionExtensions
{
    public static Mock<IGameSession> VerifyRecordMoveCalled(this Mock<IGameSession> mock, Func<Times> times)
    {
        mock.Verify(x => x.RecordMove(It.IsAny<bool>()), times);

        return mock;
    }
}