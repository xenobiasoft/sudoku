using DepenMock.Moq;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Controllers;
using Sudoku.Api.Models;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Queries;

namespace UnitTests.API;

public class ProfilesControllerTests : MoqBaseTestByType<ProfilesController>
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly ProfilesController _sut;

    public ProfilesControllerTests()
    {
        _mockMediator = Container.ResolveMock<IMediator>().AsMoq();
        _sut = ResolveSut();
    }

    #region CreateProfileAsync Tests

    [Fact]
    public async Task CreateProfileAsync_WithValidRequest_Returns201Created()
    {
        // Arrange
        var profileDto = CreateTestProfileDto();
        var request = new CreateProfileRequest("TestPlayer");

        _mockMediator
            .Setup(x => x.Send(It.IsAny<CreateProfileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProfileDto>.Success(profileDto));

        // Act
        var result = await _sut.CreateProfileAsync(request);

        // Assert
        var statusResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        statusResult.Value.Should().BeEquivalentTo(profileDto);
    }

    [Fact]
    public async Task CreateProfileAsync_WhenCommandFails_Returns400BadRequest()
    {
        // Arrange
        var errorMessage = "Alias is invalid";
        var request = new CreateProfileRequest("bad!");

        _mockMediator
            .Setup(x => x.Send(It.IsAny<CreateProfileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProfileDto>.Failure(errorMessage));

        // Act
        var result = await _sut.CreateProfileAsync(request);

        // Assert
        var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task CreateProfileAsync_WhenAliasTaken_Returns409Conflict()
    {
        // Arrange
        var errorMessage = "Alias is already taken";
        var request = new CreateProfileRequest("ExistingPlayer");

        _mockMediator
            .Setup(x => x.Send(It.IsAny<CreateProfileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProfileDto>.Failure(errorMessage, ProfileErrorCodes.AliasTaken));

        // Act
        var result = await _sut.CreateProfileAsync(request);

        // Assert
        var conflictResult = result.Result.Should().BeOfType<ConflictObjectResult>().Subject;
        conflictResult.Value.Should().Be(errorMessage);
    }

    #endregion

    #region GetProfileAsync Tests

    [Fact]
    public async Task GetProfileAsync_WhenProfileFound_Returns200Ok()
    {
        // Arrange
        var alias = "TestPlayer";
        var profileDto = CreateTestProfileDto(alias);

        _mockMediator
            .Setup(x => x.Send(It.IsAny<GetProfileByAliasQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProfileDto?>.Success(profileDto));

        // Act
        var result = await _sut.GetProfileAsync(alias);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(profileDto);
    }

    [Fact]
    public async Task GetProfileAsync_WhenProfileNotFound_Returns404NotFound()
    {
        // Arrange
        var alias = "UnknownPlayer";

        _mockMediator
            .Setup(x => x.Send(It.IsAny<GetProfileByAliasQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProfileDto?>.Success(null));

        // Act
        var result = await _sut.GetProfileAsync(alias);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetProfileAsync_WhenQueryFails_Returns400BadRequest()
    {
        // Arrange
        var alias = "TestPlayer";
        var errorMessage = "Query failed";

        _mockMediator
            .Setup(x => x.Send(It.IsAny<GetProfileByAliasQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProfileDto?>.Failure(errorMessage));

        // Act
        var result = await _sut.GetProfileAsync(alias);

        // Assert
        var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be(errorMessage);
    }

    #endregion

    #region UpdateProfileAliasAsync Tests

    [Fact]
    public async Task UpdateProfileAliasAsync_WithValidRequest_Returns200Ok()
    {
        // Arrange
        var alias = "TestPlayer";
        var updatedProfileDto = CreateTestProfileDto("NewAlias");
        var request = new UpdateProfileAliasRequest("NewAlias");

        _mockMediator
            .Setup(x => x.Send(It.IsAny<GetProfileByAliasQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProfileDto?>.Success(CreateTestProfileDto(alias)));

        _mockMediator
            .Setup(x => x.Send(It.IsAny<UpdateProfileAliasCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProfileDto>.Success(updatedProfileDto));

        // Act
        var result = await _sut.UpdateProfileAliasAsync(alias, request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(updatedProfileDto);
    }

    [Fact]
    public async Task UpdateProfileAliasAsync_WhenGetQueryFails_Returns400BadRequest()
    {
        // Arrange
        var alias = "TestPlayer";
        var errorMessage = "Query failed";
        var request = new UpdateProfileAliasRequest("NewAlias");

        _mockMediator
            .Setup(x => x.Send(It.IsAny<GetProfileByAliasQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProfileDto?>.Failure(errorMessage));

        // Act
        var result = await _sut.UpdateProfileAliasAsync(alias, request);

        // Assert
        var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task UpdateProfileAliasAsync_WhenProfileNotFoundByAlias_Returns404NotFound()
    {
        // Arrange
        var alias = "UnknownPlayer";
        var request = new UpdateProfileAliasRequest("NewAlias");

        _mockMediator
            .Setup(x => x.Send(It.IsAny<GetProfileByAliasQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProfileDto?>.Success(null));

        // Act
        var result = await _sut.UpdateProfileAliasAsync(alias, request);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task UpdateProfileAliasAsync_WhenAliasTaken_Returns409Conflict()
    {
        // Arrange
        var alias = "TestPlayer";
        var errorMessage = "New alias is already taken";
        var request = new UpdateProfileAliasRequest("TakenAlias");

        _mockMediator
            .Setup(x => x.Send(It.IsAny<GetProfileByAliasQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProfileDto?>.Success(CreateTestProfileDto(alias)));

        _mockMediator
            .Setup(x => x.Send(It.IsAny<UpdateProfileAliasCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProfileDto>.Failure(errorMessage, ProfileErrorCodes.AliasTaken));

        // Act
        var result = await _sut.UpdateProfileAliasAsync(alias, request);

        // Assert
        var conflictResult = result.Result.Should().BeOfType<ConflictObjectResult>().Subject;
        conflictResult.Value.Should().Be(errorMessage);
    }

    [Fact]
    public async Task UpdateProfileAliasAsync_WhenProfileNotFoundDuringUpdate_Returns404NotFound()
    {
        // Arrange
        var alias = "TestPlayer";
        var errorMessage = "Profile not found";
        var request = new UpdateProfileAliasRequest("NewAlias");

        _mockMediator
            .Setup(x => x.Send(It.IsAny<GetProfileByAliasQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProfileDto?>.Success(CreateTestProfileDto(alias)));

        _mockMediator
            .Setup(x => x.Send(It.IsAny<UpdateProfileAliasCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProfileDto>.Failure(errorMessage, ProfileErrorCodes.NotFound));

        // Act
        var result = await _sut.UpdateProfileAliasAsync(alias, request);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task UpdateProfileAliasAsync_WhenUpdateFails_Returns400BadRequest()
    {
        // Arrange
        var alias = "TestPlayer";
        var errorMessage = "Update failed";
        var request = new UpdateProfileAliasRequest("NewAlias");

        _mockMediator
            .Setup(x => x.Send(It.IsAny<GetProfileByAliasQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProfileDto?>.Success(CreateTestProfileDto(alias)));

        _mockMediator
            .Setup(x => x.Send(It.IsAny<UpdateProfileAliasCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProfileDto>.Failure(errorMessage));

        // Act
        var result = await _sut.UpdateProfileAliasAsync(alias, request);

        // Assert
        var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be(errorMessage);
    }

    #endregion

    #region DeleteProfileAsync Tests

    [Fact]
    public async Task DeleteProfileAsync_WhenProfileExists_Returns204NoContent()
    {
        // Arrange
        var alias = "TestPlayer";

        _mockMediator
            .Setup(x => x.Send(It.IsAny<DeleteProfileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _sut.DeleteProfileAsync(alias);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteProfileAsync_WhenProfileNotFound_Returns404NotFound()
    {
        // Arrange
        var alias = "UnknownPlayer";

        _mockMediator
            .Setup(x => x.Send(It.IsAny<DeleteProfileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure("Profile not found", ProfileErrorCodes.NotFound));

        // Act
        var result = await _sut.DeleteProfileAsync(alias);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteProfileAsync_WhenCommandFails_Returns400BadRequest()
    {
        // Arrange
        var alias = "TestPlayer";
        var errorMessage = "Delete failed unexpectedly";

        _mockMediator
            .Setup(x => x.Send(It.IsAny<DeleteProfileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(errorMessage));

        // Act
        var result = await _sut.DeleteProfileAsync(alias);

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be(errorMessage);
    }

    #endregion

    private static ProfileDto CreateTestProfileDto(string alias = "TestPlayer")
    {
        return new ProfileDto(Guid.NewGuid().ToString(), alias, DateTime.UtcNow, DateTime.UtcNow);
    }
}
