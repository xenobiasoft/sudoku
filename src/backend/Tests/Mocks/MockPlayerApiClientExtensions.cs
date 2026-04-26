using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.HttpClients;

namespace UnitTests.Mocks;

public static class MockPlayerApiClientExtensions
{
    extension(Mock<IPlayerApiClient> mock)
    {
        public void SetupCreatePlayerAsync(string alias)
        {
            var result = ApiResult<string>.Success(alias);
            mock
                .Setup(x => x.CreatePlayerAsync(It.IsAny<string>()))
                .ReturnsAsync(result);
        }

        public void SetupCreatePlayerAsyncFails()
        {
            var failureResult = ApiResult<string>.Failure("API Error");
            mock
                .Setup(x => x.CreatePlayerAsync(It.IsAny<string>()))
                .ReturnsAsync(failureResult);
        }

        public void SetupPlayerExistsAsync(bool exists)
        {
            var result = ApiResult<bool>.Success(exists);
            mock
                .Setup(x => x.PlayerExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(result);
        }

        public void SetupPlayerExistsAsyncFails()
        {
            var result = ApiResult<bool>.Failure("API Error");
            mock
                .Setup(x => x.PlayerExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(result);
        }
    }
}