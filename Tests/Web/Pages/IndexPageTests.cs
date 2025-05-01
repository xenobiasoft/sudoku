using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Services;
using XenobiaSoft.Sudoku.GameState;
using IndexPage = Sudoku.Web.Server.Pages.Index;

namespace UnitTests.Web.Pages;

public class IndexPageTests : TestContext
{
    private readonly Mock<ILocalStorageService> _mockLocalStorageService = new();

    public IndexPageTests()
    {
        Services.AddSingleton(_mockLocalStorageService.Object);
    }

    [Fact]
    public void RendersCorrectly()
    {
        // Arrange
        var component = RenderComponent<IndexPage>();

        // Act
        var startNewGameButton = component.Find("button:contains('Start New Game')");
        var loadGameButton = component.Find("button:contains('Load Game')");

        // Assert
        Assert.NotNull(startNewGameButton);
        Assert.NotNull(loadGameButton);
    }

    [Fact]
    public void ShowsDifficultyOptions_WhenStartNewGameClicked()
    {
        // Arrange
        var component = RenderComponent<IndexPage>();

        // Act
        component.Find("button:contains('Start New Game')").Click();
        var difficultyButtons = component.FindAll("button:contains('Easy'), button:contains('Medium'), button:contains('Hard')");

        // Assert
        Assert.Equal(3, difficultyButtons.Count);
    }

    [Fact]
    public void ShowsSavedGames_WhenLoadGameClicked()
    {
        // Arrange
        var savedGames = new List<GameStateMemory>
        {
            new() { PuzzleId = Guid.NewGuid().ToString(), LastUpdated = DateTime.Now.AddMinutes(-10) },
            new() { PuzzleId = Guid.NewGuid().ToString(), LastUpdated = DateTime.Now.AddMinutes(-5) }
        };
        _mockLocalStorageService
            .Setup(x => x.LoadGameStatesAsync())
            .ReturnsAsync(savedGames);

        var component = RenderComponent<IndexPage>();

        // Act
        component.Find("button:contains('Load Game')").Click();

        // Assert
        var savedGameButtons = component.FindAll("button:contains('Saved:')");
        savedGameButtons.Count.Should().Be(savedGames.Count);
    }
}