using Sudoku.Blazor.Services.Abstractions;

namespace UnitTests.Mocks;

public static class MockPlayerManagerExtensions
{
    extension(Mock<IPlayerManager> mock)
    {
        public void SetupGetCurrentPlayerAsync(string alias)
        {
            mock.Setup(x => x.GetCurrentPlayerAsync())
                .ReturnsAsync(alias);
        }
    }
}