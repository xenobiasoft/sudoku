using Sudoku.Web.Server.Components;

namespace UnitTests.Web.Components;

public class VictoryDisplayTests : TestContext
{
    [Fact]
    public void VictoryMessageIsDisplayedWhenIsVictoryIsTrue()
    {
        // Arrange
        var isVictory = true;
        var elapsedTime = new TimeSpan(0, 30, 0);

        // Act
        var sut = RenderComponent<VictoryDisplay>(parameters => parameters
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
        var sut = RenderComponent<VictoryDisplay>(parameters => parameters
            .Add(p => p.IsVictory, isVictory)
            .Add(p => p.ElapsedTime, elapsedTime)
        );

        // Assert
        sut.Find(".victory-subtitle").MarkupMatches("<p class=\"victory-subtitle\">Time: 1:02:03</p>");
    }

    [Fact]
    public void NewGameButtonIsRenderedAndClickable()
    {
        // Arrange
        var isVictory = true;
        var newGameCalled = false;
        void NewGame() => newGameCalled = true;
        var sut = RenderComponent<VictoryDisplay>(parameters => parameters
            .Add(p => p.IsVictory, isVictory)
            .Add(p => p.NewGame, NewGame)
        );
        var button = sut.Find("button.victory-button");

        // Act
        button.Click();

        // Assert
        newGameCalled.Should().BeTrue();
    }
}