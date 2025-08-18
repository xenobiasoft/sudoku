using DepenMock.XUnit;
using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.HttpClients;
using UnitTests.Helpers.Mocks;

namespace UnitTests.Web.Services;

public class AliasServiceTests : BaseTestByAbstraction<AliasService, IAliasService>
{
    private const string ExistingAlias = "test-alias";
    private readonly Mock<ILocalStorageService> _mockLocalStorageService;
    private readonly Mock<IPlayerApiClient> _mockPlayerApiClient;

    public AliasServiceTests()
    {
        _mockLocalStorageService = Container.ResolveMock<ILocalStorageService>();
        _mockPlayerApiClient = Container.ResolveMock<IPlayerApiClient>();

        _mockPlayerApiClient
            .Setup(x => x.PlayerExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(ApiResult<bool>.Success(true));
    }

    [Fact]
    public async Task GetAliasAsync_WhenAliasDoesNotExist_GeneratesAndSavesNewAlias()
    {
        // Arrange
        _mockLocalStorageService.SetupGetAliasAsync(null);
        var sut = ResolveSut();

        // Act
        var result = await sut.GetAliasAsync();

        // Assert
        result.Should().NotBeNullOrEmpty();
        _mockLocalStorageService.Verify(x => x.SetAliasAsync(result), Times.Once);
    }

    [Fact]
    public async Task GetAliasAsync_WhenAliasExists_ReturnsExistingAlias()
    {
        // Arrange
        _mockLocalStorageService.SetupGetAliasAsync(ExistingAlias);
        var sut = ResolveSut();

        // Act
        var result = await sut.GetAliasAsync();

        // Assert
        result.Should().Be(ExistingAlias);
        _mockLocalStorageService.VerifySetAliasAsyncCalled(Times.Never);
    }
}