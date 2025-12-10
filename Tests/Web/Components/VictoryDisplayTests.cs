using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Components;

namespace UnitTests.Web.Components;

public class VictoryDisplayTests : TestContext
{
    private readonly Mock<NavigationManager> _mockNavigationManager = new();

    public VictoryDisplayTests()
    {
        Services.AddSingleton(_mockNavigationManager.Object);
    }

    [Fact]
    public void VictoryMessageIsDisplayedWhenIsVictoryIsTrue()
    {
        // Arrange
        var isVictory = true;
        var elapsedTime = new TimeSpan(0, 30, 0);

        // Act
        var sut = Render<VictoryDisplay>(parameters => parameters
            .Add(p => p.IsVictory, isVictory)
            .Add(p => p.ElapsedTime, elapsedTime)
        );

        // Assert
        sut.Find(".victory-title").MarkupMatches("<h1 class=\"victory-title\">You Won!</h1>");
    }

    [Fact]
    public void ElapsedTimeIsDisplayedCorrectly()
    {
        // Arrange
        var isVictory = true;
        var elapsedTime = new TimeSpan(1, 2, 3);

        // Act
        var sut = Render<VictoryDisplay>(parameters => parameters
            .Add(p => p.IsVictory, isVictory)
            .Add(p => p.ElapsedTime, elapsedTime)
        );

        // Assert
        sut.Find(".victory-subtitle").MarkupMatches("<p class=\"victory-subtitle\">Time: 1:02:03</p>");
    }
}