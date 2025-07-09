using DepenMock.XUnit;
using Sudoku.Api.Controllers;
using System.Net;
using UnitTests.Helpers;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku.Services;

namespace UnitTests.API;

public class PlayersControllerTests : BaseTestByType<PlayersController>
{
    [Fact]
    public async Task Post_ReturnsStatusCode201()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var response = await sut.PostAsync();

        // Assert
        response.AssertResponseStatusCode(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Post_CallsPlayerService()
    {
        // Arrange
        var mockPlayerService = Container.ResolveMock<IPlayerService>();
        var sut = ResolveSut();

        // Act
        await sut.PostAsync();

        // Assert
        mockPlayerService.VerifyCreateNewCalled(Times.Once);
    }
}