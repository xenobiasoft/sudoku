using DepenMock.Moq;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Handlers;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Queries;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using UnitTests.Helpers.Factories;

namespace UnitTests.Application.Handlers;

public class GetPlayerGamesQueryHandlerTests : MoqBaseTestByAbstraction<GetPlayerGamesQueryHandler, IQueryHandler<GetPlayerGamesQuery, List<GameDto>>>
{
    private readonly Mock<IGameRepository> _mockGameRepository;

    public GetPlayerGamesQueryHandlerTests()
    {
        _mockGameRepository = Container.ResolveMock<IGameRepository>().AsMoq();
    }

    [Fact]
    public async Task Handle_WithValidProfileId_ReturnsSuccessWithGameDtos()
    {
        // Arrange
        var profileId = Guid.NewGuid().ToString();
        var games = new List<SudokuGame> { GameFactory.CreateGameInProgress(), GameFactory.CreateGameInProgress() };
        var query = new GetPlayerGamesQuery(profileId);

        _mockGameRepository.SetupGetByProfileId(games);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithValidProfileId_ReturnsEmptyListWhenNoGames()
    {
        // Arrange
        var profileId = Guid.NewGuid().ToString();
        var query = new GetPlayerGamesQuery(profileId);

        _mockGameRepository.SetupGetByProfileId([]);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithInvalidProfileId_ReturnsFailure()
    {
        // Arrange
        var query = new GetPlayerGamesQuery("invalid-guid");

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WhenDomainExceptionThrown_ReturnsFailure()
    {
        // Arrange
        var profileId = Guid.NewGuid().ToString();
        var query = new GetPlayerGamesQuery(profileId);
        var domainException = new InvalidPlayerAliasException("Invalid profile");

        _mockGameRepository.SetupThrowsOnGetByProfileId(domainException);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(domainException.Message);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ReturnsFailure()
    {
        // Arrange
        var profileId = Guid.NewGuid().ToString();
        var query = new GetPlayerGamesQuery(profileId);
        var exceptionMessage = "Database connection failed";

        _mockGameRepository.SetupThrowsOnGetByProfileId(new Exception(exceptionMessage));

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be($"An unexpected error occurred: {exceptionMessage}");
    }
}
