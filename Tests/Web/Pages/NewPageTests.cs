using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Pages;
using Sudoku.Web.Server.Services.Abstractions.V2;

namespace UnitTests.Web.Pages;

public class NewPageTests : TestContext
{
    private readonly Mock<IGameManager>? _mockGameManager;

    public NewPageTests()
    {
        var alias = "game-alias";
        _mockGameManager = new Mock<IGameManager>();
        var playerManager = new Mock<IPlayerManager>();
        playerManager.SetupGetCurrentPlayerAsync(alias);

        _mockGameManager.SetupCreateGameAsync(alias, "Medium");

        Services.AddSingleton(_mockGameManager.Object);
        Services.AddSingleton(playerManager.Object);
    }

    [Fact]
    public void OnInitializedAsync_GeneratesNewPuzzle()
    {
        // Arrange

        // Act
        RenderComponent<New>(parameters => parameters.Add(p => p.Difficulty, "Medium"));

        // Assert
        _mockGameManager!.VerifyCreateGameAsyncCalled("game-alias", "Medium", Times.Once);
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