using Sudoku.Web.Server.Services.Abstractions.V2;

namespace UnitTests.Mocks;

public static class MockPlayerManagerExtensions
{
    public static void SetupGetCurrentPlayerAsync(this Mock<IPlayerManager> mock, string alias)
    {
        mock.Setup(x => x.GetCurrentPlayerAsync())
            .ReturnsAsync(alias);
    }
}