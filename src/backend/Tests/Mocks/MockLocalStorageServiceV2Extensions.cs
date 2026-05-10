using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.Abstractions;

namespace UnitTests.Mocks;

public static class MockLocalStorageServiceV2Extensions
{
    extension(Mock<ILocalStorageService> mock)
    {
        public void SetupGetProfile(ProfileInfo profile)
        {
            mock.Setup(x => x.GetProfileAsync()).ReturnsAsync(profile);
        }

        public void SetupGetProfileReturnsNull()
        {
            mock.Setup(x => x.GetProfileAsync()).ReturnsAsync((ProfileInfo?)null);
        }

        public void VerifySavesProfile(ProfileInfo profile)
        {
            mock.Verify(x => x.SetProfileAsync(It.Is<ProfileInfo>(p => p.Alias == profile.Alias && p.ProfileId == profile.ProfileId)), Times.Once);
        }
    }
}