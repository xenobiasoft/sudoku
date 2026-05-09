using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.Abstractions;
using GameListPage = Sudoku.Blazor.Components.Pages.GameList;

namespace UnitTests.Blazor.Pages;

public class GameListPageTests : BunitContext
{
    private readonly Mock<ILocalStorageService> _mockLocalStorage = new();
    private readonly Mock<IGameManager> _mockGameManager = new();
    private const string Alias = "test-alias";

    public GameListPageTests()
    {
        Services.AddSingleton(_mockLocalStorage.Object);
        Services.AddSingleton(_mockGameManager.Object);
        Services.AddSingleton(new Mock<ILogger<GameListPage>>().Object);
    }

    private void SetupReturningPlayer()
    {
        var profile = new ProfileInfo { ProfileId = Guid.NewGuid().ToString(), Alias = Alias };
        _mockLocalStorage.Setup(x => x.GetProfileAsync()).ReturnsAsync(profile);
    }

    private void SetupNewPlayer()
    {
        _mockLocalStorage.Setup(x => x.GetProfileAsync()).ReturnsAsync((ProfileInfo?)null);
    }

    [Fact]
    public void Render_ReturningPlayer_WithGames_ShowsGameThumbnails()
    {
        SetupReturningPlayer();
        var games = new List<GameModel>
        {
            new() { Id = Guid.NewGuid().ToString(), ProfileId = Alias },
            new() { Id = Guid.NewGuid().ToString(), ProfileId = Alias },
        };
        _mockGameManager.SetupLoadGamesAsync(games);

        var component = Render<GameListPage>();

        component.FindAll(".del-game-icon").Count.Should().Be(2);
    }

    [Fact]
    public void Render_ReturningPlayer_NoGames_ShowsEmptyState()
    {
        SetupReturningPlayer();
        _mockGameManager.SetupLoadGamesAsync([]);

        var component = Render<GameListPage>();

        component.Markup.Should().Contain("No saved games yet");
    }

    [Fact]
    public void DeleteGame_RemovesGameFromList()
    {
        SetupReturningPlayer();
        var games = new List<GameModel>
        {
            new() { Id = Guid.NewGuid().ToString(), ProfileId = Alias },
            new() { Id = Guid.NewGuid().ToString(), ProfileId = Alias },
        };
        _mockGameManager.SetupLoadGamesAsync(games);

        var component = Render<GameListPage>();
        component.Find(".del-game-icon").Click();

        component.FindAll(".del-game-icon").Count.Should().Be(1);
    }

    [Fact]
    public void DeleteGame_CallsGameManagerDelete()
    {
        SetupReturningPlayer();
        var games = new List<GameModel>
        {
            new() { Id = Guid.NewGuid().ToString(), ProfileId = Alias },
        };
        _mockGameManager.SetupLoadGamesAsync(games);

        var component = Render<GameListPage>();
        component.Find(".del-game-icon").Click();

        _mockGameManager.VerifyDeleteGameAsyncCalled(Times.Once);
    }

    [Fact]
    public void NewPlayer_RedirectsToHome()
    {
        SetupNewPlayer();
        var navMan = Services.GetRequiredService<NavigationManager>();
        Render<GameListPage>();
        navMan.Uri.Should().EndWith("/");
    }

    [Fact]
    public void ClickBack_NavigatesToHome()
    {
        SetupReturningPlayer();
        _mockGameManager.SetupLoadGamesAsync([]);
        var navMan = Services.GetRequiredService<NavigationManager>();
        var component = Render<GameListPage>();
        component.Find("button:contains('← Back')").Click();
        navMan.Uri.Should().EndWith("/");
    }
}
