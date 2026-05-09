using DepenMock.Moq;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Handlers;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Queries;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Factories;

namespace UnitTests.Application.Handlers;

public class GetPlayerGamesByStatusQueryHandlerTests : MoqBaseTestByAbstraction<GetPlayerGamesByStatusQueryHandler, IQueryHandler<GetPlayerGamesByStatusQuery, List<GameDto>>>
{
    private readonly Mock<IGameRepository> _mockGameRepository;

    public GetPlayerGamesByStatusQueryHandlerTests()
    {
        _mockGameRepository = Container.ResolveMock<IGameRepository>().AsMoq();
    }

    [Fact]
    public async Task Handle_WithValidProfileIdAndStatus_ReturnsSuccessWithFilteredGames()
    {
        // Arrange
        var profileId = Guid.NewGuid().ToString();
        var games = new List<SudokuGame> { GameFactory.CreateGameInProgress(), GameFactory.CreateGameInProgress() };
        var query = new GetPlayerGamesByStatusQuery(profileId, "InProgress");

        _mockGameRepository.SetupGetByProfileIdAndStatus(games);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithValidStatusReturnsEmptyList_WhenNoGamesFound()
    {
        // Arrange
        var profileId = Guid.NewGuid().ToString();
        var query = new GetPlayerGamesByStatusQuery(profileId, "Completed");

        _mockGameRepository.SetupGetByProfileIdAndStatus([]);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithInvalidStatusString_ReturnsFailure()
    {
        // Arrange
        var profileId = Guid.NewGuid().ToString();
        var invalidStatus = "NotAValidStatus";
        var query = new GetPlayerGamesByStatusQuery(profileId, invalidStatus);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be($"Invalid game statusEnum: {invalidStatus}");
    }

    [Fact]
    public async Task Handle_WithInvalidProfileId_ReturnsFailure()
    {
        // Arrange
        var query = new GetPlayerGamesByStatusQuery("invalid-guid", "InProgress");

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_StatusParsing_IsCaseInsensitive()
    {
        // Arrange
        var profileId = Guid.NewGuid().ToString();
        var query = new GetPlayerGamesByStatusQuery(profileId, "inprogress");

        _mockGameRepository
            .Setup(x => x.GetByProfileIdAndStatusAsync(It.IsAny<ProfileId>(), GameStatusEnum.InProgress))
            .ReturnsAsync([]);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
