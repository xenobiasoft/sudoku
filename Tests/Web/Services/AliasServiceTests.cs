using DepenMock.XUnit;
using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.Abstractions;
using UnitTests.Helpers;
using UnitTests.Helpers.Mocks;

namespace UnitTests.Web.Services;

public class AliasServiceTests : BaseTestByAbstraction<AliasService, IAliasService>
{
    private const string ExistingAlias = "test-alias";
    private readonly Mock<ILocalStorageService> _mockLocalStorageService;

    public AliasServiceTests()
    {
        _mockLocalStorageService = Container.ResolveMock<ILocalStorageService>();
    }

    [Fact]
    public async Task GetAliasAsync_WhenAliasExists_ReturnsExistingAlias()
    {
        // Arrange
        _mockLocalStorageService.Setup(x => x.GetAliasAsync()).ReturnsAsync(ExistingAlias);
        var sut = ResolveSut();

        // Act
        var result = await sut.GetAliasAsync();

        // Assert
        result.Should().Be(ExistingAlias);
        _mockLocalStorageService.Verify(x => x.SetAliasAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetAliasAsync_WhenAliasDoesNotExist_GeneratesAndSavesNewAlias()
    {
        // Arrange
        _mockLocalStorageService.Setup(x => x.GetAliasAsync()).ReturnsAsync((string)null);
        var sut = ResolveSut();

        // Act
        var result = await sut.GetAliasAsync();

        // Assert
        result.Should().NotBeNullOrEmpty();
        _mockLocalStorageService.Verify(x => x.SetAliasAsync(result), Times.Once);
    }
}