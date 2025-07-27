using DepenMock.XUnit;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Handlers;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Builders;

namespace UnitTests.Application.Handlers;

public class ResetGameCommandHandlerTests : BaseTestByAbstraction<ResetGameCommandHandler, ICommandHandler<ResetGameCommand>>
{
    private readonly Mock<IGameRepository> _mockGameRepository;
    private readonly SudokuGame _game;
    private readonly ResetGameCommand _cmd;

    public ResetGameCommandHandlerTests()
    {
        _mockGameRepository = Container.ResolveMock<IGameRepository>();

        _cmd = new ResetGameCommand(GameId.New());
        _game = GetStartedGame();
        _mockGameRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ReturnsAsync(_game);
    }

    protected override void AddContainerCustomizations(Container container)
    {
        container.AddCustomizations(new CellGenerator());
        container.AddCustomizations(new GameDifficultyGenerator());
        container.AddCustomizations(new PlayerAliasGenerator());
    }

    [Fact]
    public async Task Handle_CallsSaveAsyncWithUpdatedGame()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        await sut.Handle(_cmd, CancellationToken.None);

        // Assert
        _mockGameRepository.Verify(x => x.SaveAsync(_game), Times.Once);
    }

    [Fact]
    public async Task Handle_LogsInformationWhenSuccessful()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        await sut.Handle(_cmd, CancellationToken.None);

        // Assert
        Logger.InformationLogs().ContainsMessage("Reset game with ID");
    }

    [Fact]
    public async Task Handle_WhenGameInInvalidState_ReturnsFailureResult()
    {
        // Arrange
        _mockGameRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ReturnsAsync(Container.Create<SudokuGame>());
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(_cmd, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Game is already in its initial state");
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ReturnsFailureResult()
    {
        // Arrange
        var exceptionMessage = "Database error";
        _mockGameRepository
            .Setup(x => x.SaveAsync(It.IsAny<SudokuGame>()))
            .ThrowsAsync(new Exception(exceptionMessage));
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(_cmd, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain(exceptionMessage);
    }

    [Fact]
    public async Task Handle_WithNonExistentGameId_ReturnsFailureResult()
    {
        // Arrange
        _mockGameRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<GameId>()))
            .ReturnsAsync((SudokuGame)null);
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(_cmd, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Game not found");
    }

    [Fact]
    public async Task Handle_WithValidGameId_ReturnsSuccessResult()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(_cmd, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    private SudokuGame GetStartedGame()
    {
        var game = Container.Create<SudokuGame>();
        game.StartGame();

        return game;
    }
}