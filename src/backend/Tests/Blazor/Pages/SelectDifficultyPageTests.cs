using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.Abstractions;
using SelectDifficultyPage = Sudoku.Blazor.Components.Pages.SelectDifficulty;

namespace UnitTests.Blazor.Pages;

public class SelectDifficultyPageTests : BunitContext
{
    private readonly Mock<ILocalStorageService> _mockLocalStorage = new();

    public SelectDifficultyPageTests()
    {
        Services.AddSingleton(_mockLocalStorage.Object);
        Services.AddSingleton(new Mock<ILogger<SelectDifficultyPage>>().Object);
    }

    private void SetupReturningPlayer(string alias = "test-alias")
    {
        var profile = new ProfileInfo { ProfileId = Guid.NewGuid().ToString(), Alias = alias };
        _mockLocalStorage.Setup(x => x.GetProfileAsync()).ReturnsAsync(profile);
    }

    private void SetupNewPlayer()
    {
        _mockLocalStorage.Setup(x => x.GetProfileAsync()).ReturnsAsync((ProfileInfo?)null);
    }

    [Fact]
    public void Render_ShowsThreeDifficultyButtons()
    {
        SetupReturningPlayer();
        var component = Render<SelectDifficultyPage>();
        Assert.NotNull(component.Find("button:contains('Easy')"));
        Assert.NotNull(component.Find("button:contains('Medium')"));
        Assert.NotNull(component.Find("button:contains('Hard')"));
    }

    [Fact]
    public void ClickEasy_NavigatesToNewEasyRoute()
    {
        SetupReturningPlayer();
        var navMan = Services.GetRequiredService<NavigationManager>();
        var component = Render<SelectDifficultyPage>();
        component.Find("button:contains('Easy')").Click();
        navMan.Uri.Should().EndWith("/new/Easy");
    }

    [Fact]
    public void ClickMedium_NavigatesToNewMediumRoute()
    {
        SetupReturningPlayer();
        var navMan = Services.GetRequiredService<NavigationManager>();
        var component = Render<SelectDifficultyPage>();
        component.Find("button:contains('Medium')").Click();
        navMan.Uri.Should().EndWith("/new/Medium");
    }

    [Fact]
    public void ClickHard_NavigatesToNewHardRoute()
    {
        SetupReturningPlayer();
        var navMan = Services.GetRequiredService<NavigationManager>();
        var component = Render<SelectDifficultyPage>();
        component.Find("button:contains('Hard')").Click();
        navMan.Uri.Should().EndWith("/new/Hard");
    }

    [Fact]
    public void ClickBack_NavigatesToHome()
    {
        SetupReturningPlayer();
        var navMan = Services.GetRequiredService<NavigationManager>();
        var component = Render<SelectDifficultyPage>();
        component.Find("button:contains('← Back')").Click();
        navMan.Uri.Should().EndWith("/");
    }

    [Fact]
    public void NewPlayer_RedirectsToHome()
    {
        SetupNewPlayer();
        var navMan = Services.GetRequiredService<NavigationManager>();
        Render<SelectDifficultyPage>();
        navMan.Uri.Should().EndWith("/");
    }
}
