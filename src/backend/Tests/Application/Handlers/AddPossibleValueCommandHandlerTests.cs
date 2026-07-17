using DepenMock.Attributes;
using DepenMock.Moq;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Handlers;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Exceptions;

namespace UnitTests.Application.Handlers;

[LogOutput(LogOutputTiming.Always)]
public class AddPossibleValueCommandHandlerTests : MoqBaseTestByAbstraction<AddPossibleValueCommandHandler, ICommandHandler<AddPossibleValueCommand>>
{
    private readonly Mock<IGameRepository> _mockGameRepository;

    public AddPossibleValueCommandHandlerTests()
    {
        _mockGameRepository = Container.ResolveMock<IGameRepository>().AsMoq();
    }

    [Fact]
    public async Task Handle_WhenGameFound_ReturnsSuccessResult()
    {
        // Arrange
        var gameId = Guid.NewGuid().ToString();
        var command = new AddPossibleValueCommand(gameId, 0, 0, 5);

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
        var command = new AddPossibleValueCommand(gameId, 0, 0, 5);

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
        var command = new AddPossibleValueCommand("invalid-guid", 0, 0, 5);

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
        var command = new AddPossibleValueCommand(gameId, 0, 0, 5);
        var domainException = new GameNotInProgressException("Cannot add possible value in NotStarted state");

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
        var command = new AddPossibleValueCommand(gameId, 0, 0, 5);
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
