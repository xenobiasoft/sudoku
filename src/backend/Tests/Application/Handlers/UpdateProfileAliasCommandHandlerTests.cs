using DepenMock.Attributes;
using DepenMock.Moq;
using DepenMock.XUnit.V3.Attributes;
using Sudoku.Application.Commands;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Handlers;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using LogOutput = DepenMock.XUnit.V3.Attributes.LogOutputAttribute;

namespace UnitTests.Application.Handlers;

[LogOutput(LogOutputTiming.Always)]
public class UpdateProfileAliasCommandHandlerTests : MoqBaseTestByAbstraction<UpdateProfileAliasCommandHandler, ICommandHandler<UpdateProfileAliasCommand, ProfileDto>>
{
    private readonly Mock<IUserProfileRepository> _mockProfileRepository;
    private readonly Mock<IGameRepository> _mockGameRepository;

    public UpdateProfileAliasCommandHandlerTests()
    {
        _mockProfileRepository = Container.ResolveMock<IUserProfileRepository>().AsMoq();
        _mockGameRepository = Container.ResolveMock<IGameRepository>().AsMoq();
    }

    [Fact]
    public async Task Handle_WithValidNewAlias_ReturnsSuccess()
    {
        var existingAlias = PlayerAlias.Create("oldalias");
        var profile = UserProfile.Create(existingAlias);

        _mockProfileRepository.Setup(x => x.GetByIdAsync(It.IsAny<ProfileId>())).ReturnsAsync(profile);
        _mockProfileRepository.Setup(x => x.AliasExistsAsync(It.IsAny<PlayerAlias>())).ReturnsAsync(false);
        _mockProfileRepository.Setup(x => x.SaveAsync(It.IsAny<UserProfile>())).Returns(Task.CompletedTask);

        var sut = ResolveSut();

        var result = await sut.Handle(new UpdateProfileAliasCommand(profile.Id.ToString(), "newalias"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Alias.Should().Be("newalias");
    }

    [Fact]
    public async Task Handle_WhenNewAliasTaken_ReturnsFailure()
    {
        var existingAlias = PlayerAlias.Create("oldalias");
        var profile = UserProfile.Create(existingAlias);

        _mockProfileRepository.Setup(x => x.GetByIdAsync(It.IsAny<ProfileId>())).ReturnsAsync(profile);
        _mockProfileRepository.Setup(x => x.AliasExistsAsync(It.IsAny<PlayerAlias>())).ReturnsAsync(true);

        var sut = ResolveSut();

        var result = await sut.Handle(new UpdateProfileAliasCommand(profile.Id.ToString(), "takenalias"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already taken");
        _mockProfileRepository.Verify(x => x.SaveAsync(It.IsAny<UserProfile>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenProfileNotFound_ReturnsFailure()
    {
        _mockProfileRepository.Setup(x => x.GetByIdAsync(It.IsAny<ProfileId>())).ReturnsAsync((UserProfile?)null);

        var sut = ResolveSut();

        var result = await sut.Handle(new UpdateProfileAliasCommand(Guid.NewGuid().ToString(), "newalias"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_WithInvalidNewAlias_ReturnsFailure()
    {
        var sut = ResolveSut();

        var result = await sut.Handle(new UpdateProfileAliasCommand(Guid.NewGuid().ToString(), ""), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        _mockProfileRepository.Verify(x => x.GetByIdAsync(It.IsAny<ProfileId>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PreservesOriginalAliasCase()
    {
        var existingAlias = PlayerAlias.Create("oldalias");
        var profile = UserProfile.Create(existingAlias);

        _mockProfileRepository.Setup(x => x.GetByIdAsync(It.IsAny<ProfileId>())).ReturnsAsync(profile);
        _mockProfileRepository.Setup(x => x.AliasExistsAsync(It.IsAny<PlayerAlias>())).ReturnsAsync(false);
        _mockProfileRepository.Setup(x => x.SaveAsync(It.IsAny<UserProfile>())).Returns(Task.CompletedTask);

        var sut = ResolveSut();

        var result = await sut.Handle(new UpdateProfileAliasCommand(profile.Id.ToString(), "NewAlias"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Alias.Should().Be("NewAlias");
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsUnexpectedException_ReturnsFailure()
    {
        var existingAlias = PlayerAlias.Create("oldalias");
        var profile = UserProfile.Create(existingAlias);

        _mockProfileRepository.Setup(x => x.GetByIdAsync(It.IsAny<ProfileId>())).ReturnsAsync(profile);
        _mockProfileRepository.Setup(x => x.AliasExistsAsync(It.IsAny<PlayerAlias>())).ReturnsAsync(false);
        _mockProfileRepository.Setup(x => x.SaveAsync(It.IsAny<UserProfile>()))
            .ThrowsAsync(new Exception("Storage failure"));

        var sut = ResolveSut();

        var result = await sut.Handle(new UpdateProfileAliasCommand(profile.Id.ToString(), "newalias"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("An unexpected error occurred");
    }

    [Fact]
    public async Task Handle_WithValidUpdate_SavesProfileToRepository()
    {
        var existingAlias = PlayerAlias.Create("oldalias");
        var profile = UserProfile.Create(existingAlias);

        _mockProfileRepository.Setup(x => x.GetByIdAsync(It.IsAny<ProfileId>())).ReturnsAsync(profile);
        _mockProfileRepository.Setup(x => x.AliasExistsAsync(It.IsAny<PlayerAlias>())).ReturnsAsync(false);
        _mockProfileRepository.Setup(x => x.SaveAsync(It.IsAny<UserProfile>())).Returns(Task.CompletedTask);

        var sut = ResolveSut();

        await sut.Handle(new UpdateProfileAliasCommand(profile.Id.ToString(), "newalias"), CancellationToken.None);

        _mockProfileRepository.Verify(x => x.SaveAsync(It.Is<UserProfile>(p => p.Alias.Value == "newalias")), Times.Once);
    }
}
