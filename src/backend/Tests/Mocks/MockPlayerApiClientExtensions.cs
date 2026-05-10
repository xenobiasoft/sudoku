using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.HttpClients;

namespace UnitTests.Mocks;

public static class MockPlayerApiClientExtensions
{
    extension(Mock<IPlayerApiClient> mock)
    {
        public void SetupCreateProfile(ProfileDto dto)
        {
            var apiResult = ApiResult<ProfileDto>.Success(dto);
            mock.Setup(x => x.CreateProfileAsync(It.IsAny<string>()))
                .ReturnsAsync(apiResult);
        }

        public void SetupCreateProfileFailure(string error = "Failed to create profile", int statusCode = 500)
        {
            mock.Setup(x => x.CreateProfileAsync(It.IsAny<string>()))
                .ReturnsAsync(ApiResult<ProfileDto>.Failure(error, statusCode));
        }

        public void SetupGetProfile(ProfileDto dto)
        {
            mock.Setup(x => x.GetProfileAsync(It.IsAny<string>()))
                .ReturnsAsync(ApiResult<ProfileDto?>.Success(dto));
        }

        public void SetupGetProfileReturnsNull()
        {
            mock.Setup(x => x.GetProfileAsync(It.IsAny<string>()))
                .ReturnsAsync(ApiResult<ProfileDto?>.Success(null));
        }

        public void SetupGetProfileFailure(string error = "Failed to get profile", int statusCode = 500)
        {
            mock.Setup(x => x.GetProfileAsync(It.IsAny<string>()))
                .ReturnsAsync(ApiResult<ProfileDto?>.Failure(error, statusCode));
        }

        public void SetupUpdateProfileAlias(ProfileDto dto)
        {
            mock.Setup(x => x.UpdateProfileAliasAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(ApiResult<ProfileDto>.Success(dto));
        }

        public void SetupUpdateProfileAliasFailure(string error = "Failed to update alias", int statusCode = 500)
        {
            mock.Setup(x => x.UpdateProfileAliasAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(ApiResult<ProfileDto>.Failure(error, statusCode));
        }

        public void VerifyCreatesProfile(string displayName)
        {
            mock.Verify(x => x.CreateProfileAsync(displayName), Times.Once);
        }
    }
}