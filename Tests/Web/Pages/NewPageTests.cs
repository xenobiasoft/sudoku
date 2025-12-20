using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudoku.Web.Server.Pages;
using Sudoku.Web.Server.Services.Abstractions;

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
        
        // Add IWebHostEnvironment mock for error boundary
        var mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
        mockWebHostEnvironment.Setup(x => x.EnvironmentName).Returns("Test");
        Services.AddSingleton(mockWebHostEnvironment.Object);
        
        // Add logger mocks
        Services.AddSingleton(new Mock<ILogger<New>>().Object);
        Services.AddSingleton(new Mock<ILogger<Sudoku.Web.Server.Components.NewGameErrorBoundary>>().Object);
    }

    [Fact]
    public void OnInitializedAsync_GeneratesNewPuzzle()
    {
        // Arrange

        // Act
        Render<New>(parameters => parameters.Add(p => p.Difficulty, "Medium"));

        // Assert
        _mockGameManager!.VerifyCreateGameAsyncCalled("game-alias", "Medium", Times.Once);
    }

    [Fact]
    public void OnInitializedAsync_NavigatesToGamePageWithPuzzleId()
    {
        // Arrange
        var navMan = Services.GetRequiredService<NavigationManager>();

        // Act
        Render<New>(parameters => parameters.Add(p => p.Difficulty, "Medium"));

        // Assert
        navMan.Uri.Should().StartWith("http://localhost/game/");
    }

    [Fact]
    public void RendersLoader()
    {
        // Arrange

        // Act
        var sut = Render<New>(parameters => parameters.Add(p => p.Difficulty, "Medium"));

        // Assert
        var loader = sut.Find("div.sudoku-loader");
        loader.Should().NotBeNull();
    }
}