using DepenMock.Moq;
using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services;
using Sudoku.Blazor.Services.Abstractions;
using Sudoku.Blazor.Services.HttpClients;

namespace UnitTests.Blazor.Services;

public class PlayerManagerTests : MoqBaseTestByAbstraction<PlayerManager, IPlayerManager>
{
    private readonly Mock<ILocalStorageService> _mockLocalStorageService;
    private readonly Mock<IPlayerApiClient> _mockPlayerApiClient;
    private readonly IPlayerManager _sut;

    public PlayerManagerTests()
    {
        _mockLocalStorageService = Container.ResolveMock<ILocalStorageService>().AsMoq();
        _mockPlayerApiClient = Container.ResolveMock<IPlayerApiClient>().AsMoq();

        _sut = ResolveSut();
    }

    [Fact]
    public async Task CreateProfileAsync_CallsApiClient()
    {
        // Arrange
        var displayName = Container.Create<string>();

        // Act
        await _sut.CreateProfileAsync(displayName);

        // Assert
        _mockPlayerApiClient.VerifyCreatesProfile(displayName);
    }

    [Fact]
    public async Task CreateProfileAsync_WhenCreateProfileSuccess_SavesToLocalStorage()
    {
        // Arrange
        var profile = Container.Create<ProfileInfo>();
        var profileDto = Container.Build<ProfileDto>()
            .With(x => x.Alias, profile.Alias)
            .With(x => x.ProfileId, profile.ProfileId)
            .Create();
        _mockPlayerApiClient.SetupCreateProfile(profileDto);

        // Act
        await _sut.CreateProfileAsync(profile.Alias);

        // Assert
        _mockLocalStorageService.VerifySavesProfile(profile);
    }

    [Fact]
    public async Task CreateProfileAsync_WhenApiClientReturnsFailure_ReturnsFailureResult()
    {
        // Arrange
        var errorMessage = Container.Create<string>();
        _mockPlayerApiClient.SetupCreateProfileFailure(errorMessage);

        // Act
        var result = await _sut.CreateProfileAsync(Container.Create<string>());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public async Task GetCurrentProfileAsync_ReturnsProfileFromLocalStorage()
    {
        // Arrange
        var profile = Container.Create<ProfileInfo>();
        _mockLocalStorageService.SetupGetProfile(profile);

        // Act
        var result = await _sut.GetCurrentProfileAsync();

        // Assert
        result.Should().BeEquivalentTo(profile);
    }

    [Fact]
    public async Task GetCurrentProfileAsync_WhenNoProfileInStorage_ReturnsNull()
    {
        // Arrange
        _mockLocalStorageService.SetupGetProfileReturnsNull();

        // Act
        var result = await _sut.GetCurrentProfileAsync();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task EnsureProfileInitializedAsync_WhenNoProfileInStorage_ReturnsFalse()
    {
        // Arrange
        _mockLocalStorageService.SetupGetProfileReturnsNull();

        // Act
        var result = await _sut.EnsureProfileInitializedAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task EnsureProfileInitializedAsync_WhenProfileExistsInStorageAndBackend_ReturnsTrue()
    {
        // Arrange
        var profile = Container.Create<ProfileInfo>();
        var profileDto = Container.Build<ProfileDto>()
            .With(x => x.Alias, profile.Alias)
            .With(x => x.ProfileId, profile.ProfileId)
            .Create();
        _mockLocalStorageService.SetupGetProfile(profile);
        _mockPlayerApiClient.SetupGetProfile(profileDto);

        // Act
        var result = await _sut.EnsureProfileInitializedAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task EnsureProfileInitializedAsync_WhenProfileInStorageButNotBackend_RecreatesProfileAndReturnsTrue()
    {
        // Arrange
        var profile = Container.Create<ProfileInfo>();
        var profileDto = Container.Build<ProfileDto>()
            .With(x => x.Alias, profile.Alias)
            .With(x => x.ProfileId, profile.ProfileId)
            .Create();
        _mockLocalStorageService.SetupGetProfile(profile);
        _mockPlayerApiClient.SetupGetProfileReturnsNull();
        _mockPlayerApiClient.SetupCreateProfile(profileDto);

        // Act
        var result = await _sut.EnsureProfileInitializedAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task EnsureProfileInitializedAsync_WhenBackendUnavailable_ThrowsInvalidOperationException()
    {
        // Arrange
        var profile = Container.Create<ProfileInfo>();
        var errorMessage = Container.Create<string>();
        _mockLocalStorageService.SetupGetProfile(profile);
        _mockPlayerApiClient.SetupGetProfileFailure(errorMessage);

        // Act
        var act = async () => await _sut.EnsureProfileInitializedAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"*{errorMessage}*");
    }

    [Fact]
    public async Task UpdateAliasAsync_WhenNoProfileInStorage_ReturnsFailureResult()
    {
        // Arrange
        _mockLocalStorageService.SetupGetProfileReturnsNull();

        // Act
        var result = await _sut.UpdateAliasAsync(Container.Create<string>());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Profile not found");
    }

    [Fact]
    public async Task UpdateAliasAsync_WhenUpdateSucceeds_UpdatesLocalStorageAndReturnsSuccess()
    {
        // Arrange
        var profile = Container.Create<ProfileInfo>();
        var newAlias = Container.Create<string>();
        var updatedDto = Container.Build<ProfileDto>()
            .With(x => x.ProfileId, profile.ProfileId)
            .With(x => x.Alias, newAlias)
            .Create();
        _mockLocalStorageService.SetupGetProfile(profile);
        _mockPlayerApiClient.SetupUpdateProfileAlias(updatedDto);

        // Act
        var result = await _sut.UpdateAliasAsync(newAlias);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Alias.Should().Be(newAlias);
        _mockLocalStorageService.VerifySavesProfile(result.Value);
    }

    [Fact]
    public async Task UpdateAliasAsync_WhenApiClientReturnsFailure_ReturnsFailureResult()
    {
        // Arrange
        var profile = Container.Create<ProfileInfo>();
        var errorMessage = Container.Create<string>();
        _mockLocalStorageService.SetupGetProfile(profile);
        _mockPlayerApiClient.SetupUpdateProfileAliasFailure(errorMessage);

        // Act
        var result = await _sut.UpdateAliasAsync(Container.Create<string>());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(errorMessage);
    }
}