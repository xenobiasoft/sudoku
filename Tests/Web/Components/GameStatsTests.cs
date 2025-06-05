using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.Services.Abstractions;
using UnitTests.Helpers.Mocks;

namespace UnitTests.Web.Components;

public class GameStatsTests : TestContext
{
    private readonly Mock<IGameTimer> _mockTimer;
    private readonly Mock<IGameSession> _mockSession;

    public GameStatsTests()
    {
        _mockSession = new Mock<IGameSession>();
        _mockTimer = new Mock<IGameTimer>();
        _mockSession.SetupGet(x => x.Timer).Returns(_mockTimer.Object);
        _mockSession.SetupGet(x => x.IsNull).Returns(false);
        Services.AddSingleton(_mockTimer.Object);
    }

    [Fact]
    public void GameStats_WhenCollapsed_RendersCorrectly()
    {
        // Arrange
        var gameStats = RenderComponent<GameStats>(x => x.Add(c => c.Session, _mockSession.Object));
        _mockSession.Setup(x => x.PlayDuration).Returns(TimeSpan.FromMinutes(15));

        // Act
        _mockTimer.RaiseTick();

        // Assert
        gameStats.MarkupMatches(@$"
            <div class=""game-stats"">
              <div class=""stat-header"">
                <span class=""label"">Play Duration:</span>
                <span class=""value"">00:15:00</span>
                <i class=""fa-solid fa-chevron-up""></i>
              </div>
            </div>");
    }

    [Fact]
    public async Task GameStats_WhenExpanded_RendersCorrectly()
    {
        // Arrange
        var gameStats = RenderComponent<GameStats>(x => x.Add(c => c.Session, _mockSession.Object));
        var statHeader = gameStats.Find(".stat-header");
        _mockSession.Setup(x => x.PlayDuration).Returns(TimeSpan.FromMinutes(15));
        _mockSession.Setup(x => x.TotalMoves).Returns(2);
        _mockSession.Setup(x => x.InvalidMoves).Returns(1);
        _mockTimer.RaiseTick();

        // Act
        await gameStats.InvokeAsync(() => statHeader.Click());

        // Assert
        gameStats.MarkupMatches(@$"
            <div class=""game-stats"">
              <div class=""stat-header"">
                <span class=""label"">Play Duration:</span>
                <span class=""value"">00:15:00</span>
                <i class=""fa-solid fa-chevron-down""></i>
              </div>
              <div class=""stat-item total-moves"">
                <span class=""label"">Total Moves:</span>
                <span class=""value"">2</span>
              </div>
              <div class=""stat-item invalid-moves"">
                <span class=""label"">Invalid Moves:</span>
                <span class=""value"">1</span>
              </div>
            </div>");
    }

    [Fact]
    public async Task GameStats_WhenMovesAreRecorded_ShouldUpdate()
    {
        // Arrange
        var gameStats = RenderComponent<GameStats>(x => x.Add(c => c.Session, _mockSession.Object));
        var statHeader = gameStats.Find(".stat-header");
        await gameStats.InvokeAsync(() => statHeader.Click());
        _mockSession.Setup(x => x.TotalMoves).Returns(2);
        _mockSession.Setup(x => x.InvalidMoves).Returns(1);
        _mockSession.Raise(x => x.OnMoveRecorded += null, EventArgs.Empty);

        // Act
        var totalMoves = gameStats.Find(".total-moves .value");
        var invalidMoves = gameStats.Find(".invalid-moves .value");

        // Assert
        totalMoves.MarkupMatches("<span class=\"value\">2</span>");
        invalidMoves.MarkupMatches("<span class=\"value\">1</span>");
    }

    [Fact]
    public async Task GameStats_WhenTimerTicks_ShouldUpdate()
    {
        // Arrange
        var gameStats = RenderComponent<GameStats>(x => x.Add(c => c.Session, _mockSession.Object));
        var playDuration = gameStats.Find(".game-stats .stat-header .value");
        _mockSession.Setup(x => x.PlayDuration).Returns(TimeSpan.FromMinutes(5));

        // Act
        _mockTimer.RaiseTick();

        // Assert
        playDuration.MarkupMatches("<span class=\"value\">00:05:00</span>");
    }
}