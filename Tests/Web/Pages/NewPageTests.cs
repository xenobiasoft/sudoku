using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Pages;
using Sudoku.Web.Server.Services.Abstractions;
using UnitTests.Helpers.Mocks;

namespace UnitTests.Web.Pages;

public class NewPageTests : TestContext
{
    private const string GameAlias = "test-alias";
    private readonly Mock<IAliasService>? _mockAliasService;
    private readonly Mock<IApiBasedGameStateManager>? _mockGameStateManager;

    public NewPageTests()
    {
        _mockGameStateManager = new Mock<IApiBasedGameStateManager>();
        _mockAliasService = new Mock<IAliasService>();

        _mockAliasService.SetupGetAliasAsync(GameAlias);
        _mockGameStateManager.SetupCreateGameAsync(GameAlias);

        Services.AddSingleton(_mockGameStateManager.Object);
        Services.AddSingleton(_mockAliasService.Object);
    }

    [Fact]
    public void OnInitializedAsync_CreatesGame()
    {
        // Arrange

        // Act
        RenderComponent<New>(parameters => parameters.Add(p => p.Difficulty, "Medium"));

        // Assert
        _mockGameStateManager?.VerifyCreateGameAsync(GameAlias, Times.Once);
    }

    [Fact]
    public void OnInitializedAsync_GetsAlias()
    {
        // Arrange

        // Act
        RenderComponent<New>(parameters => parameters.Add(p => p.Difficulty, "Medium"));

        // Assert
        _mockAliasService?.VerifyGetAliasAsync(Times.Once);
    }

    [Fact]
    public void OnInitializedAsync_NavigatesToGamePageWithPuzzleId()
    {
        // Arrange
        var navMan = Services.GetRequiredService<FakeNavigationManager>();

        // Act
        RenderComponent<New>(parameters => parameters.Add(p => p.Difficulty, "Medium"));

        // Assert
        navMan.Uri.Should().StartWith("http://localhost/game/");
    }

    [Fact]
    public void RendersLoader()
    {
        // Arrange

        // Act
        var sut = RenderComponent<New>(parameters => parameters.Add(p => p.Difficulty, "Medium"));

        // Assert
        var loader = sut.Find("div.sudoku-loader");
        loader.Should().NotBeNull();
    }
}