using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.HttpClients;

namespace UnitTests.Mocks;

public static class MockPlayerApiClientExtensions
{
    public static void SetupCreatePlayerAsync(this Mock<IPlayerApiClient> mock, string alias)
    {
        var result = ApiResult<string>.Success(alias);
        mock
            .Setup(x => x.CreatePlayerAsync(It.IsAny<string>()))
            .ReturnsAsync(result);
    }

    public static void SetupCreatePlayerAsyncFails(this Mock<IPlayerApiClient> mock)
    {
        var failureResult = ApiResult<string>.Failure("API Error");
        mock
            .Setup(x => x.CreatePlayerAsync(It.IsAny<string>()))
            .ReturnsAsync(failureResult);
    }

    public static void SetupPlayerExistsAsync(this Mock<IPlayerApiClient> mock, bool exists)
    {
        var result = ApiResult<bool>.Success(exists);
        mock
            .Setup(x => x.PlayerExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(result);
    }

    public static void SetupPlayerExistsAsyncFails(this Mock<IPlayerApiClient> mock)
    {
        var result = ApiResult<bool>.Failure("API Error");
        mock
            .Setup(x => x.PlayerExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(result);
    }
}