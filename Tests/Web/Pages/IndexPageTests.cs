using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.Abstractions.V2;
using IndexPage = Sudoku.Web.Server.Pages.Index;

namespace UnitTests.Web.Pages;

public class IndexPageTests : TestContext
{
    private const string Alias = "test-alias";
    private readonly Mock<IGameManager> _mockGameManager = new();
    private readonly Mock<IPlayerManager> _mockPlayerManager = new();

    public IndexPageTests()
    {
        var savedGames = new List<GameModel>
        {
            new() { Id = Guid.NewGuid().ToString(), PlayerAlias = Alias},
            new() { Id = Guid.NewGuid().ToString(), PlayerAlias = Alias},
        };
        _mockGameManager.SetupLoadGamesAsync(savedGames);
        _mockPlayerManager.SetupGetCurrentPlayerAsync(Alias);
        Services.AddSingleton(_mockGameManager.Object);
        Services.AddSingleton(_mockPlayerManager.Object);
    }

    [Fact]
    public void DeleteGame_RemovesGameFromList()
    {
        // Arrange
        var component = RenderComponent<IndexPage>();
        var delGameElement = component.Find(".del-game-icon");

        // Act
        delGameElement.Click();

        // Assert
        component.FindAll(".del-game-icon").Count.Should().Be(1);
    }

    [Fact]
    public void DeleteGame_WhenClicked_RemovesGameFromGameStateManager()
    {
        // Arrange
        var component = RenderComponent<IndexPage>();
        var delGameElement = component.Find(".del-game-icon");

        // Act
        delGameElement.Click();

        // Assert
        _mockGameManager.VerifyDeleteGameAsyncCalled(Times.Once);
    }

    [Fact]
    public void Render_WhenSavedGamesPresent_DisplaysEachSavedGame()
    {
        // Arrange
        var component = RenderComponent<IndexPage>();

        // Act
        var delGameElements = component.FindAll(".del-game-icon");

        // Assert
        delGameElements.Count.Should().Be(2);
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
        var savedGames = new List<GameModel>
        {
        };
        _mockGameManager.SetupLoadGamesAsync(savedGames);
        var component = RenderComponent<IndexPage>();

        // Act
        component.Find("button:contains('Load Game')").Click();

        // Assert
        var savedGameButtons = component.FindAll(".saved-game-card");
        savedGameButtons.Count.Should().Be(savedGames.Count);
    }
}