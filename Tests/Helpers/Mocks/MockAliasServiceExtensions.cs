using Sudoku.Web.Server.Services.Abstractions;

namespace UnitTests.Helpers.Mocks;

public static class MockAliasServiceExtensions
{
    public static void SetupGetAliasAsync(this Mock<IAliasService> mock, string alias)
    {
        mock
            .Setup(x => x.GetAliasAsync())
            .ReturnsAsync(alias);
    }

    public static void VerifyGetAliasAsync(this Mock<IAliasService> mock, Func<Times> times)
    {
        mock.Verify(x => x.GetAliasAsync(), times);
    }
}