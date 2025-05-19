using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.Services;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Web.Components;

public class GameStatsTests : TestContext
{
    private readonly Mock<IGameTimer> _mockTimer;
    private readonly GameSession _session;

    public GameStatsTests()
    {
        _mockTimer = new Mock<IGameTimer>();
        var mockSessionManager = new Mock<IGameSessionManager>();
        _session = new GameSession(new GameStateMemory("puzzle-id", []), _mockTimer.Object);

        mockSessionManager.Setup(x => x.CurrentSession).Returns(_session);
        Services.AddSingleton(mockSessionManager.Object);
    }

    [Fact]
    public void GameStats_WhenCollapsed_RendersCorrectly()
    {
        // Arrange
        _mockTimer.Setup(x => x.ElapsedTime).Returns(TimeSpan.FromMinutes(15));
        var gameStats = RenderComponent<GameStats>();

        // Act
        _session.RecordMove(true); // Total moves: 1
        _session.RecordMove(false); // Total moves: 2, Invalid moves: 1

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
    public void GameStats_WhenExpanded_RendersCorrectly()
    {
        // Arrange
        _mockTimer.Setup(x => x.ElapsedTime).Returns(TimeSpan.FromMinutes(15));
        var gameStats = RenderComponent<GameStats>();
        _session.RecordMove(true); // Total moves: 1
        _session.RecordMove(false); // Total moves: 2, Invalid moves: 1
        var statHeader = gameStats.Find(".stat-header");

        // Act
        statHeader.Click();

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
    public void GameStats_WhenMovesAreRecorded_ShouldUpdate()
    {
        // Arrange
        var gameStats = RenderComponent<GameStats>();
        var statHeader = gameStats.Find(".stat-header");
        _session.RecordMove(true);
        _session.RecordMove(false);
        statHeader.Click();

        // Act
        var totalMoves = gameStats.Find(".total-moves .value");
        var invalidMoves = gameStats.Find(".invalid-moves .value");

        // Assert
        totalMoves.MarkupMatches("<span class=\"value\">2</span>");
        invalidMoves.MarkupMatches("<span class=\"value\">1</span>");
    }

    [Fact]
    public void GameStats_WhenTimerTicks_ShouldUpdate()
    {
        // Arrange
        var gameStats = RenderComponent<GameStats>();
        _mockTimer.Setup(x => x.ElapsedTime).Returns(TimeSpan.FromMinutes(5));
        var playDuration = gameStats.Find(".game-stats .stat-header .value");

        // Act
        _mockTimer.Raise(x => x.OnTick += null, this, TimeSpan.FromMinutes(5));

        // Assert
        playDuration.MarkupMatches("<span class=\"value\">00:05:00</span>");
    }
}