using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.Abstractions;

namespace UnitTests.Mocks;

public static class MockPlayerManagerExtensions
{
    extension(Mock<IPlayerManager> mock)
    {
        public void SetupCurrentProfile(ProfileInfo profile)
        {
            mock.Setup(x => x.GetCurrentProfileAsync())
                .ReturnsAsync(profile);
        }
    }
}