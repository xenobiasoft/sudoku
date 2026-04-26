using Sudoku.Blazor.Services.Abstractions;

namespace UnitTests.Mocks;

public static class MockAliasServiceExtensions
{
    extension(Mock<IAliasService> mock)
    {
        public void SetupGetAliasAsync(string alias)
        {
            mock
                .Setup(x => x.GetAliasAsync())
                .ReturnsAsync(alias);
        }

        public void VerifyGetAliasAsync(Func<Times> times)
        {
            mock.Verify(x => x.GetAliasAsync(), times);
        }
    }
}