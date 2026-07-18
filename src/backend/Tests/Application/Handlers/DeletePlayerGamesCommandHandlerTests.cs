using DepenMock.Attributes;
using DepenMock.Moq;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Handlers;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Builders;

namespace UnitTests.Application.Handlers;

[LogOutput(LogOutputTiming.Always)]
public class DeletePlayerGamesCommandHandlerTests : MoqBaseTestByAbstraction<DeletePlayerGamesCommandHandler, ICommandHandler<DeletePlayerGamesCommand>>
{
    private readonly Mock<IGameRepository> _mockGameRepository;

    public DeletePlayerGamesCommandHandlerTests()
    {
        _mockGameRepository = Container.ResolveMock<IGameRepository>().AsMoq();
    }

    protected override void AddContainerCustomizations(Container container)
    {
        container.AddCustomizations(new PlayerAliasGenerator());
        container.AddCustomizations(new GameDifficultyGenerator());
        container.AddCustomizations(new CellGenerator());
        container.AddCustomizations(new BoardSizeGenerator());
    }

    [Fact]
    public async Task Handle_CallsDeleteAsyncForEachGame()
    {
        // Arrange
        var profileId = Guid.NewGuid().ToString();
        var command = new DeletePlayerGamesCommand(profileId);
        var games = Container.CreateMany<SudokuGame>(2);

        _mockGameRepository
            .Setup(x => x.GetByProfileIdAsync(It.IsAny<ProfileId>()))
            .ReturnsAsync(games);

        var sut = ResolveSut();

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        _mockGameRepository.Verify(x => x.DeleteAsync(It.IsAny<GameId>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_LogsDebugWithGameCount()
    {
        // Arrange
        var profileId = Guid.NewGuid().ToString();
        var command = new DeletePlayerGamesCommand(profileId);
        var games = Container.CreateMany<SudokuGame>(2);

        _mockGameRepository
            .Setup(x => x.GetByProfileIdAsync(It.IsAny<ProfileId>()))
            .ReturnsAsync(games);

        var sut = ResolveSut();

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        Logger.DebugLogs().ContainsMessage("Deleted 2 games for profile");
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ReturnsFailureResult()
    {
        // Arrange
        var profileId = Guid.NewGuid().ToString();
        var command = new DeletePlayerGamesCommand(profileId);
        var exceptionMessage = "Database error";

        _mockGameRepository
            .Setup(x => x.GetByProfileIdAsync(It.IsAny<ProfileId>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain(exceptionMessage);
    }

    [Fact]
    public async Task Handle_WhenDeleteThrowsException_ReturnsFailureResult()
    {
        // Arrange
        var profileId = Guid.NewGuid().ToString();
        var command = new DeletePlayerGamesCommand(profileId);
        var games = Container.CreateMany<SudokuGame>(1);
        var exceptionMessage = "Database error";

        _mockGameRepository
            .Setup(x => x.GetByProfileIdAsync(It.IsAny<ProfileId>()))
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
        var profileId = Guid.NewGuid().ToString();
        var command = new DeletePlayerGamesCommand(profileId);
        var games = new List<SudokuGame>();

        _mockGameRepository
            .Setup(x => x.GetByProfileIdAsync(It.IsAny<ProfileId>()))
            .ReturnsAsync(games);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithValidProfileId_ReturnsSuccessResult()
    {
        // Arrange
        var profileId = Guid.NewGuid().ToString();
        var command = new DeletePlayerGamesCommand(profileId);
        var games = Container.CreateMany<SudokuGame>(2);

        _mockGameRepository
            .Setup(x => x.GetByProfileIdAsync(It.IsAny<ProfileId>()))
            .ReturnsAsync(games);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
