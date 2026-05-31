using DepenMock.Moq;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Handlers;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Exceptions;

namespace UnitTests.Application.Handlers;

public class PauseGameCommandHandlerTests : MoqBaseTestByAbstraction<PauseGameCommandHandler, ICommandHandler<PauseGameCommand>>
{
    private readonly Mock<IGameRepository> _mockGameRepository;

    public PauseGameCommandHandlerTests()
    {
        _mockGameRepository = Container.ResolveMock<IGameRepository>().AsMoq();
    }

    [Fact]
    public async Task Handle_WhenGameFound_ReturnsSuccessResult()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var command = new PauseGameCommand(gameId);

        _mockGameRepository.SetupGameInProgress();

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockGameRepository.VerifySaveCalledOnce();
    }

    [Fact]
    public async Task Handle_WhenGameNotFound_ReturnsFailureResult()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var command = new PauseGameCommand(gameId);

        _mockGameRepository.SetupGameNotFound();

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be($"Game not found with ID: {gameId}");
        _mockGameRepository.VerifySaveNeverCalled();
    }

    [Fact]
    public async Task Handle_WithInvalidGameId_ReturnsFailureResult()
    {
        // Arrange
        var command = new PauseGameCommand("invalid-guid");

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WhenDomainExceptionThrown_ReturnsFailureWithDomainMessage()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var command = new PauseGameCommand(gameId);
        var domainException = new GameNotInProgressException("Cannot pause game in NotStarted state");

        _mockGameRepository.SetupGetByIdThrows(domainException);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(domainException.Message);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ReturnsFailureResult()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var command = new PauseGameCommand(gameId);
        var exceptionMessage = "Database connection failed";

        _mockGameRepository.SetupGetByIdThrows(new Exception(exceptionMessage));

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be($"An unexpected error occurred: {exceptionMessage}");
    }
}
