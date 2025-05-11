using Sudoku.Web.Server.Components;

namespace UnitTests.Web.Components;

public class GameStatsTests : TestContext
{
    [Fact]
    public void GameStats_ShouldRenderCorrectly_WithValidParameters()
    {
        // Arrange
        var totalMoves = 42;
        var invalidMoves = 5;
        var playDuration = TimeSpan.FromMinutes(15);

        // Act
        var cut = RenderComponent<GameStats>(parameters => parameters
            .Add(p => p.TotalMoves, totalMoves)
            .Add(p => p.InvalidMoves, invalidMoves)
            .Add(p => p.PlayDuration, playDuration)
        );

        // Assert
        cut.MarkupMatches(@$"
<div class=""game-stats"">
    <div class=""stat-item"">
        <span class=""label"">Total Moves:</span>
        <span class=""value"">{totalMoves}</span>
    </div>
    <div class=""stat-item"">
        <span class=""label"">Invalid Moves:</span>
        <span class=""value"">{invalidMoves}</span>
    </div>
    <div class=""stat-item"">
        <span class=""label"">Play Duration:</span>
        <span class=""value"">{playDuration:hh\:mm\:ss}</span>
    </div>
</div>");
    }

    [Fact]
    public void GameStats_ShouldUpdate_WhenParametersChange()
    {
        // Arrange
        var sut = RenderComponent<GameStats>(parameters => parameters
            .Add(p => p.TotalMoves, 10)
            .Add(p => p.InvalidMoves, 2)
            .Add(p => p.PlayDuration, TimeSpan.FromMinutes(5))
        );

        // Act
        sut.SetParametersAndRender(parameters => parameters
            .Add(p => p.TotalMoves, 20)
            .Add(p => p.InvalidMoves, 4)
            .Add(p => p.PlayDuration, TimeSpan.FromMinutes(10))
        );

        // Assert
        sut.Find(".game-stats .stat-item:nth-child(1) .value").MarkupMatches("<span class=\"value\">20</span>");
        sut.Find(".game-stats .stat-item:nth-child(2) .value").MarkupMatches("<span class=\"value\">4</span>");
        sut.Find(".game-stats .stat-item:nth-child(3) .value").MarkupMatches("<span class=\"value\">00:10:00</span>");
    }
}