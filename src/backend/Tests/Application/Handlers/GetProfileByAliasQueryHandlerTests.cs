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
public class GetProfileByAliasQueryHandlerTests : MoqBaseTestByAbstraction<GetProfileByAliasQueryHandler, IQueryHandler<GetProfileByAliasQuery, ProfileDto?>>
{
    private readonly Mock<IUserProfileRepository> _mockProfileRepository;

    public GetProfileByAliasQueryHandlerTests()
    {
        _mockProfileRepository = Container.ResolveMock<IUserProfileRepository>().AsMoq();
    }

    [Fact]
    public async Task Handle_WhenProfileFound_ReturnsSuccessWithDto()
    {
        // Arrange
        var alias = "TestPlayer";
        var profile = UserProfileFactory.CreateProfile(alias);
        var query = new GetProfileByAliasQuery(alias);

        _mockProfileRepository.SetupGetByAlias(profile);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Alias.Should().Be(alias);
    }

    [Fact]
    public async Task Handle_WhenProfileNotFound_ReturnsSuccessWithNullValue()
    {
        // Arrange
        var query = new GetProfileByAliasQuery("UnknownAlias");

        _mockProfileRepository.SetupGetByAliasNotFound();

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithInvalidAlias_ReturnsFailure()
    {
        // Arrange
        var query = new GetProfileByAliasQuery(string.Empty);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ReturnsFailure()
    {
        // Arrange
        var alias = "TestPlayer";
        var query = new GetProfileByAliasQuery(alias);
        var exceptionMessage = "Database connection failed";

        _mockProfileRepository.SetupThrowsOnGetByAlias(new Exception(exceptionMessage));

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be($"An unexpected error occurred: {exceptionMessage}");
    }
}
