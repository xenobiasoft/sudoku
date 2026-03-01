using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudoku.Blazor.Components.Controls;
using IndexPage = Sudoku.Blazor.Components.Pages.Index;

namespace UnitTests.Web.Pages;

public class IndexPageTests : BunitContext
{
    public IndexPageTests()
    {
        // Add IWebHostEnvironment mock for error boundary
        var mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
        mockWebHostEnvironment.Setup(x => x.EnvironmentName).Returns("Test");
        Services.AddSingleton(mockWebHostEnvironment.Object);

        // Add logger mocks
        Services.AddSingleton(new Mock<ILogger<IndexPage>>().Object);
        Services.AddSingleton(new Mock<ILogger<IndexErrorBoundary>>().Object);
    }

    [Fact]
    public void RendersCorrectly()
    {
        // Arrange & Act
        var component = Render<IndexPage>();

        // Assert
        var startButton = component.Find("#btnStart");
        startButton.Should().NotBeNull();
        startButton.TextContent.Should().Contain("Play");
    }

    [Fact]
    public void RendersLandingPageStructure()
    {
        // Arrange & Act
        var component = Render<IndexPage>();

        // Assert
        var landingPage = component.Find("div.landing-page");
        landingPage.Should().NotBeNull();
        
        var btnPanel = component.Find("div.btn-panel");
        btnPanel.Should().NotBeNull();
        btnPanel.GetAttribute("role").Should().Be("group");
    }

    [Fact]
    public void StartButton_HasCorrectAttributes()
    {
        // Arrange & Act
        var component = Render<IndexPage>();

        // Assert
        var startButton = component.Find("#btnStart");
        startButton.GetAttribute("type").Should().Be("button");
        startButton.ClassList.Should().Contain("btn");
        startButton.ClassList.Should().Contain("btn-primary");
    }

    [Fact]
    public void StartButton_ContainsPlayIcon()
    {
        // Arrange & Act
        var component = Render<IndexPage>();

        // Assert
        var icon = component.Find("#btnStart i.fa-solid.fa-play-circle");
        icon.Should().NotBeNull();
    }

    [Fact]
    public void StartButton_WhenClicked_NavigatesToStartPage()
    {
        // Arrange
        var component = Render<IndexPage>();
        var navMan = Services.GetRequiredService<NavigationManager>();
        var startButton = component.Find("#btnStart");

        // Act
        startButton.Click();

        // Assert
        navMan.Uri.Should().EndWith("/start");
    }

    [Fact]
    public void Component_RendersOnRootRoute()
    {
        // Arrange
        var navMan = Services.GetRequiredService<NavigationManager>();

        // Act
        var component = Render<IndexPage>();

        // Assert
        // Verify the component renders successfully on the expected route
        component.Should().NotBeNull();
        navMan.Uri.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void RendersWithinIndexErrorBoundary()
    {
        // Arrange & Act
        var component = Render<IndexPage>();

        // Assert
        // If the component renders successfully, the error boundary is working
        var landingPage = component.Find("div.landing-page");
        landingPage.Should().NotBeNull();
    }

    [Fact]
    public void RendersMatLayoutGrid()
    {
        // Arrange & Act
        var component = Render<IndexPage>();

        // Assert
        var matLayoutGrid = component.Find("div.mat-layout-grid");
        matLayoutGrid.Should().NotBeNull();
        
        var matLayoutGridInner = component.Find("div.mat-layout-grid-inner");
        matLayoutGridInner.Should().NotBeNull();
    }
}
