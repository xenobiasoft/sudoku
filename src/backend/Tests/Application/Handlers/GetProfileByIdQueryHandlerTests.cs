using DepenMock.Attributes;
using DepenMock.Moq;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Handlers;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Queries;
using Sudoku.Domain.Entities;
using UnitTests.Helpers.Factories;

namespace UnitTests.Application.Handlers;

[LogOutput(LogOutputTiming.Always)]
public class GetProfileByIdQueryHandlerTests : MoqBaseTestByAbstraction<GetProfileByIdQueryHandler, IQueryHandler<GetProfileByIdQuery, ProfileDto?>>
{
    private readonly Mock<IUserProfileRepository> _mockProfileRepository;

    public GetProfileByIdQueryHandlerTests()
    {
        _mockProfileRepository = Container.ResolveMock<IUserProfileRepository>().AsMoq();
    }

    [Fact]
    public async Task Handle_WithInvalidGuidFormat_ReturnsFailure()
    {
        // Arrange
        var query = new GetProfileByIdQuery("not-a-valid-guid");

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invalid ProfileId format: not-a-valid-guid");
        _mockProfileRepository.VerifyGetByIdNeverCalled();
    }

    [Fact]
    public async Task Handle_WhenProfileFound_ReturnsSuccessWithDto()
    {
        // Arrange
        var profileId = Guid.NewGuid().ToString();
        var profile = UserProfileFactory.CreateProfile();
        var query = new GetProfileByIdQuery(profileId);

        _mockProfileRepository.SetupGetById(profile);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Alias.Should().Be("TestPlayer");
    }

    [Fact]
    public async Task Handle_WhenProfileNotFound_ReturnsSuccessWithNullValue()
    {
        // Arrange
        var profileId = Guid.NewGuid().ToString();
        var query = new GetProfileByIdQuery(profileId);

        _mockProfileRepository.SetupGetByIdNotFound();

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ReturnsFailure()
    {
        // Arrange
        var profileId = Guid.NewGuid().ToString();
        var query = new GetProfileByIdQuery(profileId);
        var exceptionMessage = "Database connection failed";

        _mockProfileRepository.SetupThrowsOnGetById(new Exception(exceptionMessage));

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be($"An unexpected error occurred: {exceptionMessage}");
    }
}
