using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.Abstractions;
using IndexPage = Sudoku.Blazor.Components.Pages.Index;

namespace UnitTests.Blazor.Pages;

public class IndexPageTests : BunitContext
{
    private readonly Mock<ILocalStorageService> _mockLocalStorage = new();

    public IndexPageTests()
    {
        Services.AddSingleton(_mockLocalStorage.Object);

        var mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
        mockWebHostEnvironment.Setup(x => x.EnvironmentName).Returns("Test");
        Services.AddSingleton(mockWebHostEnvironment.Object);

        Services.AddSingleton(new Mock<ILogger<IndexPage>>().Object);
        Services.AddSingleton(new Mock<ILogger<Sudoku.Blazor.Components.IndexErrorBoundary>>().Object);
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
    public void Render_NewPlayer_ShowsCreateProfileCard()
    {
        SetupNewPlayer();
        var component = Render<IndexPage>();
        var profileButton = component.Find("button:contains('Create Profile')");
        Assert.NotNull(profileButton);
    }

    [Fact]
    public void Render_ReturningPlayer_ShowsManageProfileCard()
    {
        SetupReturningPlayer();
        var component = Render<IndexPage>();
        var profileButton = component.Find("button:contains('Manage Profile')");
        Assert.NotNull(profileButton);
    }

    [Fact]
    public void Render_NewPlayer_StartNewGameIsDisabled()
    {
        SetupNewPlayer();
        var component = Render<IndexPage>();
        var startBtn = component.Find("button:contains('Start New Game')");
        Assert.True(startBtn.HasAttribute("disabled"));
    }

    [Fact]
    public void Render_NewPlayer_BrowseGameListIsDisabled()
    {
        SetupNewPlayer();
        var component = Render<IndexPage>();
        var browseBtn = component.Find("button:contains('Browse Game List')");
        Assert.True(browseBtn.HasAttribute("disabled"));
    }

    [Fact]
    public void Render_ReturningPlayer_StartNewGameIsEnabled()
    {
        SetupReturningPlayer();
        var component = Render<IndexPage>();
        var startBtn = component.Find("button:contains('Start New Game')");
        Assert.False(startBtn.HasAttribute("disabled"));
    }

    [Fact]
    public void Render_ReturningPlayer_BrowseGameListIsEnabled()
    {
        SetupReturningPlayer();
        var component = Render<IndexPage>();
        var browseBtn = component.Find("button:contains('Browse Game List')");
        Assert.False(browseBtn.HasAttribute("disabled"));
    }

    [Fact]
    public void Render_NewPlayer_ShowsHelperTextOnDisabledCards()
    {
        SetupNewPlayer();
        var component = Render<IndexPage>();
        var helperTexts = component.FindAll(".helper-text");
        helperTexts.Count.Should().Be(2);
    }

    [Fact]
    public void Render_DoesNotCallBackendApi()
    {
        SetupNewPlayer();
        Render<IndexPage>();
        // LocalStorageService is the only service injected — no IPlayerApiClient or IGameManager
        // This test verifies no backend dependency is needed
        _mockLocalStorage.Verify(x => x.GetProfileAsync(), Times.AtLeastOnce);
    }
}
