using Sudoku.Web.Server.Services.Abstractions;

namespace UnitTests.Helpers.Mocks;

public static class MockGameSessionManagerExtensions
{
    public static Mock<IGameSessionManager> VerifyMoveRecorded(this Mock<IGameSessionManager>  mock, Func<Times> times)
    {
        mock.Verify(x => x.RecordMove(It.IsAny<bool>()), times);
        
        return mock;
    }
}