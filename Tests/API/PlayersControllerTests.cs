using DepenMock.XUnit;
using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Controllers;
using Sudoku.Application.Common;
using Sudoku.Application.Interfaces;

namespace UnitTests.API;

public class PlayersControllerTests : BaseTestByType<PlayersController>
{
    private readonly Mock<IPlayerApplicationService> _mockPlayerService;
    private readonly PlayersController _sut;

    public PlayersControllerTests()
    {
        _mockPlayerService = Container.ResolveMock<IPlayerApplicationService>();
        _sut = ResolveSut();
    }

    [Fact]
    public async Task CreatePlayerAsync_WhenServiceReturnsSuccess_ReturnsCreatedAtAction()
    {
        // Arrange
        var generatedAlias = "HappyTiger42";
        _mockPlayerService
            .Setup(x => x.CreatePlayerAsync(It.IsAny<string>()))
            .ReturnsAsync(Result<string>.Success(generatedAlias));

        // Act
        var result = await _sut.CreatePlayerAsync();

        // Assert
        var createdAtActionResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtActionResult.ActionName.Should().Be(nameof(PlayersController.PlayerExistsAsync));
        createdAtActionResult.RouteValues.Should().ContainKey("alias").WhoseValue.Should().Be(generatedAlias);
        createdAtActionResult.Value.Should().Be(generatedAlias);
    }

    [Fact]
    public async Task CreatePlayerAsync_WithCustomAlias_PassesAliasToService()
    {
        // Arrange
        var customAlias = "CustomAlias";
        var request = new CreatePlayerRequest { Alias = customAlias };
        
        _mockPlayerService
            .Setup(x => x.CreatePlayerAsync(customAlias))
            .ReturnsAsync(Result<string>.Success(customAlias))
            .Verifiable();

        // Act
        await _sut.CreatePlayerAsync(request);

        // Assert
        _mockPlayerService.Verify();
    }

    [Fact]
    public async Task CreatePlayerAsync_WhenServiceReturnsFailed_ReturnsBadRequest()
    {
        // Arrange
        var errorMessage = "Failed to create player";
        _mockPlayerService
            .Setup(x => x.CreatePlayerAsync(It.IsAny<string>()))
            .ReturnsAsync(Result<string>.Failure(errorMessage));

        // Act
        var result = await _sut.CreatePlayerAsync();

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task PlayerExistsAsync_WithValidAlias_ReturnsOkWithResult()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var exists = true;
        
        _mockPlayerService
            .Setup(x => x.PlayerExistsAsync(playerAlias))
            .ReturnsAsync(Result<bool>.Success(exists));

        // Act
        var result = await _sut.PlayerExistsAsync(playerAlias);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(exists);
    }

    [Fact]
    public async Task PlayerExistsAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Arrange
        string playerAlias = "";

        // Act
        var result = await _sut.PlayerExistsAsync(playerAlias);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Player alias cannot be null or empty.");
    }

    [Fact]
    public async Task PlayerExistsAsync_WhenServiceFails_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var errorMessage = "Failed to check player existence";
        
        _mockPlayerService
            .Setup(x => x.PlayerExistsAsync(playerAlias))
            .ReturnsAsync(Result<bool>.Failure(errorMessage));

        // Act
        var result = await _sut.PlayerExistsAsync(playerAlias);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task DeletePlayerAsync_WithExistingPlayer_ReturnsNoContent()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        
        _mockPlayerService
            .Setup(x => x.PlayerExistsAsync(playerAlias))
            .ReturnsAsync(Result<bool>.Success(true));
            
        _mockPlayerService
            .Setup(x => x.DeletePlayerAsync(playerAlias))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _sut.DeletePlayerAsync(playerAlias);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeletePlayerAsync_WithEmptyAlias_ReturnsBadRequest()
    {
        // Arrange
        string playerAlias = "";

        // Act
        var result = await _sut.DeletePlayerAsync(playerAlias);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Player alias cannot be null or empty.");
    }

    [Fact]
    public async Task DeletePlayerAsync_WhenPlayerDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var playerAlias = "NonExistentPlayer";
        
        _mockPlayerService
            .Setup(x => x.PlayerExistsAsync(playerAlias))
            .ReturnsAsync(Result<bool>.Success(false));

        // Act
        var result = await _sut.DeletePlayerAsync(playerAlias);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeletePlayerAsync_WhenExistsCheckFails_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var errorMessage = "Failed to check player existence";
        
        _mockPlayerService
            .Setup(x => x.PlayerExistsAsync(playerAlias))
            .ReturnsAsync(Result<bool>.Failure(errorMessage));

        // Act
        var result = await _sut.DeletePlayerAsync(playerAlias);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task DeletePlayerAsync_WhenDeleteFails_ReturnsBadRequest()
    {
        // Arrange
        var playerAlias = "TestPlayer";
        var errorMessage = "Failed to delete player";
        
        _mockPlayerService
            .Setup(x => x.PlayerExistsAsync(playerAlias))
            .ReturnsAsync(Result<bool>.Success(true));
            
        _mockPlayerService
            .Setup(x => x.DeletePlayerAsync(playerAlias))
            .ReturnsAsync(Result<bool>.Failure(errorMessage));

        // Act
        var result = await _sut.DeletePlayerAsync(playerAlias);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);
    }
}