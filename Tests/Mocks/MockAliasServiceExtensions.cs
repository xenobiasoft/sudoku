using Sudoku.Web.Server.Services.Abstractions;

namespace UnitTests.Mocks;

public static class MockAliasServiceExtensions
{
    public static Mock<IAliasService> SetupGetAliasAsync(this Mock<IAliasService> mock, string alias)
    {
        mock
            .Setup(x => x.GetAliasAsync())
            .ReturnsAsync(alias);

        return mock;
    }
}