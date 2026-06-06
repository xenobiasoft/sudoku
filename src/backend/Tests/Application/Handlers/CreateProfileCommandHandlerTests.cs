using DepenMock.Attributes;
using DepenMock.Moq;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Handlers;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Application.Handlers;

[LogOutput(LogOutputTiming.Always)]
public class CreateProfileCommandHandlerTests : MoqBaseTestByAbstraction<CreateProfileCommandHandler, ICommandHandler<CreateProfileCommand, ProfileDto>>
{
    private readonly Mock<IUserProfileRepository> _mockProfileRepository;

    public CreateProfileCommandHandlerTests()
    {
        _mockProfileRepository = Container.ResolveMock<IUserProfileRepository>().AsMoq();
    }

    [Fact]
    public async Task Handle_WithValidAlias_ReturnsSuccessWithProfileDto()
    {
        _mockProfileRepository.Setup(x => x.AliasExistsAsync(It.IsAny<PlayerAlias>())).ReturnsAsync(false);
        _mockProfileRepository.Setup(x => x.SaveAsync(It.IsAny<Sudoku.Domain.Entities.UserProfile>())).Returns(Task.CompletedTask);
        var sut = ResolveSut();

        var result = await sut.Handle(new CreateProfileCommand("testalias"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Alias.Should().Be("testalias");
    }

    [Fact]
    public async Task Handle_PreservesOriginalAliasCase()
    {
        _mockProfileRepository.Setup(x => x.AliasExistsAsync(It.IsAny<PlayerAlias>())).ReturnsAsync(false);
        _mockProfileRepository.Setup(x => x.SaveAsync(It.IsAny<Sudoku.Domain.Entities.UserProfile>())).Returns(Task.CompletedTask);
        var sut = ResolveSut();

        var result = await sut.Handle(new CreateProfileCommand("MixedCase"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Alias.Should().Be("MixedCase");
    }

    [Fact]
    public async Task Handle_WhenAliasTaken_ReturnsFailure()
    {
        _mockProfileRepository.Setup(x => x.AliasExistsAsync(It.IsAny<PlayerAlias>())).ReturnsAsync(true);
        var sut = ResolveSut();

        var result = await sut.Handle(new CreateProfileCommand("takenalias"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already taken");
        _mockProfileRepository.Verify(x => x.SaveAsync(It.IsAny<Sudoku.Domain.Entities.UserProfile>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidAlias_ReturnsFailure()
    {
        var sut = ResolveSut();

        var result = await sut.Handle(new CreateProfileCommand(""), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _mockProfileRepository.Verify(x => x.AliasExistsAsync(It.IsAny<PlayerAlias>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithTooShortAlias_ReturnsFailure()
    {
        var sut = ResolveSut();

        var result = await sut.Handle(new CreateProfileCommand("a"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_SavesProfileAfterValidation()
    {
        _mockProfileRepository.Setup(x => x.AliasExistsAsync(It.IsAny<PlayerAlias>())).ReturnsAsync(false);
        _mockProfileRepository.Setup(x => x.SaveAsync(It.IsAny<Sudoku.Domain.Entities.UserProfile>())).Returns(Task.CompletedTask);
        var sut = ResolveSut();

        await sut.Handle(new CreateProfileCommand("validalias"), CancellationToken.None);

        _mockProfileRepository.Verify(x => x.SaveAsync(It.IsAny<Sudoku.Domain.Entities.UserProfile>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsUnexpectedException_ReturnsFailure()
    {
        _mockProfileRepository.Setup(x => x.AliasExistsAsync(It.IsAny<PlayerAlias>())).ReturnsAsync(false);
        _mockProfileRepository.Setup(x => x.SaveAsync(It.IsAny<Sudoku.Domain.Entities.UserProfile>()))
            .ThrowsAsync(new Exception("Storage failure"));
        var sut = ResolveSut();

        var result = await sut.Handle(new CreateProfileCommand("validalias"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("An unexpected error occurred");
    }
}
