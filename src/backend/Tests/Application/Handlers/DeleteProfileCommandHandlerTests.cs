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

    public DeleteProfileCommandHandlerTests()
    {
        _mockProfileRepository = Container.ResolveMock<IUserProfileRepository>().AsMoq();
        _mockMediator = Container.ResolveMock<IMediator>().AsMoq();
    }

    protected override void AddContainerCustomizations(Container container)
    {
        container.AddCustomizations(new PlayerAliasGenerator());
    }

    private static UserProfile CreateProfile(string alias = "testplayer") =>
        UserProfile.Create(PlayerAlias.Create(alias));

    private void SetupDeleteGamesSuccess() =>
        _mockMediator
            .Setup(x => x.Send(It.IsAny<DeletePlayerGamesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

    [Fact]
    public async Task Handle_WithValidAlias_DeletesGamesAndProfileAndReturnsSuccess()
    {
        // Arrange
        var profile = CreateProfile("validplayer");
        _mockProfileRepository.Setup(x => x.GetByAliasAsync(It.IsAny<PlayerAlias>())).ReturnsAsync(profile);
        _mockProfileRepository.Setup(x => x.DeleteAsync(It.IsAny<ProfileId>())).Returns(Task.CompletedTask);
        SetupDeleteGamesSuccess();

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(new DeleteProfileCommand("validplayer"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockMediator.Verify(x => x.Send(It.IsAny<DeletePlayerGamesCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockProfileRepository.Verify(x => x.DeleteAsync(It.IsAny<ProfileId>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenProfileNotFound_ReturnsFailureWithNotFoundErrorCode()
    {
        // Arrange
        _mockProfileRepository.Setup(x => x.GetByAliasAsync(It.IsAny<PlayerAlias>())).ReturnsAsync((UserProfile?)null);

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(new DeleteProfileCommand("unknownplayer"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(ProfileErrorCodes.NotFound);
        _mockMediator.Verify(x => x.Send(It.IsAny<DeletePlayerGamesCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockProfileRepository.Verify(x => x.DeleteAsync(It.IsAny<ProfileId>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDeleteGamesFails_ReturnsFailureAndSkipsProfileDelete()
    {
        // Arrange
        var profile = CreateProfile("validplayer");
        _mockProfileRepository.Setup(x => x.GetByAliasAsync(It.IsAny<PlayerAlias>())).ReturnsAsync(profile);
        _mockMediator
            .Setup(x => x.Send(It.IsAny<DeletePlayerGamesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure("game delete failed"));

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(new DeleteProfileCommand("validplayer"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        _mockProfileRepository.Verify(x => x.DeleteAsync(It.IsAny<ProfileId>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsFailure()
    {
        // Arrange
        var exceptionMessage = "Database connection failed";
        _mockProfileRepository
            .Setup(x => x.GetByAliasAsync(It.IsAny<PlayerAlias>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        var sut = ResolveSut();

        // Act
        var result = await sut.Handle(new DeleteProfileCommand("testplayer"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain(exceptionMessage);
    }
}
