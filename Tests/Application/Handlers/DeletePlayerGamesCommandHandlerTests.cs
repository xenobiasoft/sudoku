using DepenMock.XUnit;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Handlers;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Builders;

namespace UnitTests.Application.Handlers;

public class DeletePlayerGamesCommandHandlerTests : BaseTestByAbstraction<DeletePlayerGamesCommandHandler, ICommandHandler<DeletePlayerGamesCommand>>
{
    private readonly Mock<IGameRepository> _mockGameRepository;

    public DeletePlayerGamesCommandHandlerTests()
    {
        _mockGameRepository = Container.ResolveMock<IGameRepository>();
    }

    protected override void AddContainerCustomizations(Container container)
    {
        container.AddCustomizations(new PlayerAliasGenerator());
        container.AddCustomizations(new GameDifficultyGenerator());
        container.AddCustomizations(new CellGenerator());
    }

    [Fact]
    public async Task Handle_CallsDeleteAsyncForEachGame()
    {
        // Arrange
        var playerAlias = Container.Create<PlayerAlias>();
        var command = new DeletePlayerGamesCommand(playerAlias);
        var games = Container.CreateMany<SudokuGame>(2);

        _mockGameRepository
            .Setup(x => x.GetByPlayerAsync(It.IsAny<PlayerAlias>()))
            .ReturnsAsync(games);

        var sut = ResolveSut();

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        _mockGameRepository.Verify(x => x.DeleteAsync(It.IsAny<GameId>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_LogsInformationWithGameCount()
    {
        // Arrange
        var playerAlias = Container.Create<PlayerAlias>();
        var command = new DeletePlayerGamesCommand(playerAlias);
        var games = Container.CreateMany<SudokuGame>(2);

        _mockGameRepository
            .Setup(x => x.GetByPlayerAsync(It.IsAny<PlayerAlias>()))
            .ReturnsAsync(games);

        var sut = ResolveSut();

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        Logger.InformationLogs().ContainsMessage($"Deleted 2 games for player {playerAlias}");
    }

    [Fact]
    public async Task Handle_WhenDomainExceptionThrown_ReturnsFailureResult()
    {
        // Arrange
        var playerAlias = Container.Create<PlayerAlias>();
        var command = new DeletePlayerGamesCommand(playerAlias);
        var exceptionMessage = "Invalid player alias format";

        _mockGameRepository
            .Setup(x => x.GetByPlayerAsync(It.IsAny<PlayerAlias>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain(exceptionMessage);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ReturnsFailureResult()
    {
        // Arrange
        var playerAlias = Container.Create<PlayerAlias>();
        var command = new DeletePlayerGamesCommand(playerAlias);
        var games = Container.CreateMany<SudokuGame>(1);
        var exceptionMessage = "Database error";

        _mockGameRepository
            .Setup(x => x.GetByPlayerAsync(It.IsAny<PlayerAlias>()))
            .ReturnsAsync(games);

        _mockGameRepository
            .Setup(x => x.DeleteAsync(It.IsAny<GameId>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain(exceptionMessage);
    }

    [Fact]
    public async Task Handle_WithNoGames_ReturnsSuccessResult()
    {
        // Arrange
        var playerAlias = Container.Create<PlayerAlias>();
        var command = new DeletePlayerGamesCommand(playerAlias);
        var games = new List<SudokuGame>();

        _mockGameRepository
            .Setup(x => x.GetByPlayerAsync(It.IsAny<PlayerAlias>()))
            .ReturnsAsync(games);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithValidPlayerAlias_ReturnsSuccessResult()
    {
        // Arrange
        var playerAlias = Container.Create<PlayerAlias>();
        var command = new DeletePlayerGamesCommand(playerAlias);
        var games = Container.CreateMany<SudokuGame>(2);

        _mockGameRepository
            .Setup(x => x.GetByPlayerAsync(It.IsAny<PlayerAlias>()))
            .ReturnsAsync(games);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}