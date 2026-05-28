using DepenMock.Attributes;
using DepenMock.Moq;
using MediatR;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.Handlers;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Builders;

namespace UnitTests.Application.Handlers;

[LogOutput(LogOutputTiming.Always)]
public class DeleteProfileCommandHandlerTests : MoqBaseTestByAbstraction<DeleteProfileCommandHandler, ICommandHandler<DeleteProfileCommand>>
{
    private readonly Mock<IUserProfileRepository> _mockProfileRepository;
    private readonly Mock<IMediator> _mockMediator;

    private readonly ICommandHandler<DeleteProfileCommand> _sut;

    public DeleteProfileCommandHandlerTests()
    {
        _mockProfileRepository = Container.ResolveMock<IUserProfileRepository>().AsMoq();
        _mockMediator = Container.ResolveMock<IMediator>().AsMoq();

        _sut = ResolveSut();
    }

    protected override void AddContainerCustomizations(Container container)
    {
        container.AddCustomizations(new PlayerAliasGenerator());
    }

    [Fact]
    public async Task Handle_WithValidAlias_DeletesGamesAndProfileAndReturnsSuccess()
    {
        // Arrange
        var command = new DeleteProfileCommand("validplayer");
        var profile = CreateProfile("validplayer");
        _mockProfileRepository.SetupGetByAlias(profile);
        SetupDeleteGamesSuccess();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockMediator.Verify(x => x.Send(It.IsAny<DeletePlayerGamesCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockProfileRepository.VerifyDeleteAsyncCalled(Times.Once());
    }

    [Fact]
    public async Task Handle_WhenProfileNotFound_ReturnsFailureWithNotFoundErrorCode()
    {
        // Arrange
        var command = new DeleteProfileCommand("unknownplayer");
        _mockProfileRepository.SetupGetByAliasNotFound();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ProfileErrorCodes.NotFound);
        _mockMediator.Verify(x => x.Send(It.IsAny<DeletePlayerGamesCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockProfileRepository.VerifyDeleteAsyncCalled(Times.Never());
    }

    [Fact]
    public async Task Handle_WhenDeleteGamesFails_ReturnsFailureAndSkipsProfileDelete()
    {
        // Arrange
        var command = new DeleteProfileCommand("validplayer");
        var profile = CreateProfile("validplayer");
        _mockProfileRepository.SetupGetByAlias(profile);
        _mockMediator
            .Setup(x => x.Send(It.IsAny<DeletePlayerGamesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure("game delete failed"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        _mockProfileRepository.VerifyDeleteAsyncCalled(Times.Never());
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsFailure()
    {
        // Arrange
        var command = new DeleteProfileCommand("testplayer");
        var exceptionMessage = "Database connection failed";
        _mockProfileRepository.SetupThrowsOnGetByAlias(new Exception(exceptionMessage));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain(exceptionMessage);
    }

    private static UserProfile CreateProfile(string alias = "testplayer") => UserProfile.Create(PlayerAlias.Create(alias));

    private void SetupDeleteGamesSuccess() =>
        _mockMediator
            .Setup(x => x.Send(It.IsAny<DeletePlayerGamesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
}
