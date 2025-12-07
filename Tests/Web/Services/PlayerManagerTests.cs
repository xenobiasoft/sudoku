using DepenMock.XUnit;
using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.HttpClients;
using ILocalStorageService = Sudoku.Web.Server.Services.Abstractions.ILocalStorageService;
using IPlayerManager = Sudoku.Web.Server.Services.Abstractions.IPlayerManager;

namespace UnitTests.Web.Services;

public class PlayerManagerTests : BaseTestByAbstraction<PlayerManager, IPlayerManager>
{
    private const string TestAlias = "TestPlayer";
    private const string CreatedAlias = "CreatedPlayer";
    
    private readonly Mock<IPlayerApiClient> _mockPlayerApiClient;
    private readonly Mock<ILocalStorageService> _mockLocalStorageService;

    public PlayerManagerTests()
    {
        _mockPlayerApiClient = Container.ResolveMock<IPlayerApiClient>();
        _mockLocalStorageService = Container.ResolveMock<ILocalStorageService>();
    }

    [Fact]
    public async Task CreatePlayerAsync_WhenApiCallFails_ThrowsException()
    {
        // Arrange
        _mockPlayerApiClient.SetupCreatePlayerAsyncFails();
        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.CreatePlayerAsync(TestAlias);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Failed to create player.");
    }

    [Fact]
    public async Task CreatePlayerAsync_WhenApiReturnsNullValue_ThrowsException()
    {
        // Arrange
        _mockPlayerApiClient.SetupCreatePlayerAsync(null);
        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.CreatePlayerAsync(TestAlias);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Failed to create player.");
    }

    [Fact]
    public async Task CreatePlayerAsync_WithNullAlias_CallsApiWithNull()
    {
        // Arrange
        var expectedAlias = CreatedAlias;
        _mockPlayerApiClient.SetupCreatePlayerAsync(expectedAlias);
        var sut = ResolveSut();

        // Act
        var result = await sut.CreatePlayerAsync(null);

        // Assert
        result.Should().Be(expectedAlias);
        _mockPlayerApiClient.Verify(x => x.CreatePlayerAsync(null), Times.Once);
    }

    [Fact]
    public async Task CreatePlayerAsync_WithValidAlias_ReturnsCreatedAlias()
    {
        // Arrange
        var expectedAlias = CreatedAlias;
        _mockPlayerApiClient.SetupCreatePlayerAsync(expectedAlias);
        var sut = ResolveSut();

        // Act
        var result = await sut.CreatePlayerAsync(TestAlias);

        // Assert
        result.Should().Be(expectedAlias);
    }

    [Fact]
    public async Task CreatePlayerAsync_WithValidAlias_StoresAliasInLocalStorage()
    {
        // Arrange
        var expectedAlias = CreatedAlias;
        _mockPlayerApiClient.SetupCreatePlayerAsync(expectedAlias);
        var sut = ResolveSut();

        // Act
        await sut.CreatePlayerAsync(TestAlias);

        // Assert
        _mockLocalStorageService.Verify(x => x.SetAliasAsync(expectedAlias), Times.Once);
    }

    [Fact]
    public async Task GetCurrentPlayerAsync_ReturnsAliasFromLocalStorage()
    {
        // Arrange
        var expectedAlias = TestAlias;
        _mockLocalStorageService.SetupGetAliasAsync(expectedAlias);
        var sut = ResolveSut();

        // Act
        var result = await sut.GetCurrentPlayerAsync();

        // Assert
        result.Should().Be(expectedAlias);
    }

    [Fact]
    public async Task PlayerExistsAsync_WhenApiCallFails_ThrowsException()
    {
        // Arrange
        _mockPlayerApiClient.SetupPlayerExistsAsyncFails();
        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.PlayerExistsAsync(TestAlias);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Failed to check if player exists.");
    }

    [Fact]
    public async Task GetCurrentPlayerAsync_WhenNoAliasStored_ReturnsNull()
    {
        // Arrange
        _mockLocalStorageService.SetupGetAliasAsync(null);
        var sut = ResolveSut();

        // Act
        var result = await sut.GetCurrentPlayerAsync();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task PlayerExistsAsync_WithEmptyAlias_ThrowsArgumentException()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.PlayerExistsAsync(string.Empty);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Alias not set.");
    }

    [Fact]
    public async Task PlayerExistsAsync_WithNullAlias_ThrowsArgumentException()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.PlayerExistsAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Alias not set.");
    }

    [Fact]
    public async Task PlayerExistsAsync_WithValidAlias_ReturnsFalse()
    {
        // Arrange
        _mockPlayerApiClient.SetupPlayerExistsAsync(false);
        var sut = ResolveSut();

        // Act
        var result = await sut.PlayerExistsAsync(TestAlias);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task PlayerExistsAsync_WithValidAlias_ReturnsTrue()
    {
        // Arrange
        _mockPlayerApiClient.SetupPlayerExistsAsync(true);
        var sut = ResolveSut();

        // Act
        var result = await sut.PlayerExistsAsync(TestAlias);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task SetCurrentPlayerAsync_WithEmptyAlias_ThrowsArgumentException()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.SetCurrentPlayerAsync(string.Empty);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Alias not set.");
    }

    [Fact]
    public async Task SetCurrentPlayerAsync_WithNullAlias_ThrowsArgumentException()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        Func<Task> act = async () => await sut.SetCurrentPlayerAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Alias not set.");
    }

    [Fact]
    public async Task SetCurrentPlayerAsync_WithValidAlias_CallsLocalStorageService()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        await sut.SetCurrentPlayerAsync(TestAlias);

        // Assert
        _mockLocalStorageService.Verify(x => x.SetAliasAsync(TestAlias), Times.Once);
    }
}