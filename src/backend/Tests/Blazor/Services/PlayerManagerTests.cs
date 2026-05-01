using DepenMock.Moq;
using Sudoku.Blazor.Services;
using Sudoku.Blazor.Services.Abstractions;

namespace UnitTests.Blazor.Services;

public class PlayerManagerTests : MoqBaseTestByAbstraction<PlayerManager, IPlayerManager>
{
    private const string TestAlias = "TestPlayer";

    private readonly Mock<ILocalStorageService> _mockLocalStorageService;

    public PlayerManagerTests()
    {
        _mockLocalStorageService = Container.ResolveMock<ILocalStorageService>().AsMoq();
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