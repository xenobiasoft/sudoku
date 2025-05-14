using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.Services;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Web.Components;

public class GameStatsTests : TestContext
{
    private readonly Mock<IGameSessionManager> _mockSessionManager;
    private readonly Mock<IGameTimer> _mockTimer;
    private readonly GameSession _session;

    public GameStatsTests()
    {
        _mockTimer = new Mock<IGameTimer>();
        _mockSessionManager = new Mock<IGameSessionManager>();
        _session = new GameSession(new GameStateMemory("puzzle-id", []), _mockTimer.Object);

        _mockSessionManager.Setup(x => x.CurrentSession).Returns(_session);
        Services.AddSingleton(_mockSessionManager.Object);
    }

    [Fact]
    public void GameStats_ShouldRenderCorrectly_WithValidSession()
    {
        // Arrange
        _session.RecordMove(true); // Total moves: 1
        _session.RecordMove(false); // Total moves: 2, Invalid moves: 1
        _mockTimer.Setup(x => x.ElapsedTime).Returns(TimeSpan.FromMinutes(15));

        // Act
        var cut = RenderComponent<GameStats>();

        // Assert
        cut.MarkupMatches(@$"
<div class=""game-stats"">
    <div class=""stat-item"">
        <span class=""label"">Total Moves:</span>
        <span class=""value"">2</span>
    </div>
    <div class=""stat-item"">
        <span class=""label"">Invalid Moves:</span>
        <span class=""value"">1</span>
    </div>
    <div class=""stat-item"">
        <span class=""label"">Play Duration:</span>
        <span class=""value"">00:15:00</span>
    </div>
</div>");
    }

    [Fact]
    public void GameStats_ShouldUpdate_WhenTimerTicks()
    {
        // Arrange
        var cut = RenderComponent<GameStats>();
        _mockTimer.Setup(x => x.ElapsedTime).Returns(TimeSpan.FromMinutes(5));

        // Act
        _mockTimer.Raise(x => x.OnTick += null, this, TimeSpan.FromMinutes(5));

        // Assert
        cut.Find(".game-stats .stat-item:nth-child(3) .value").MarkupMatches("<span class=\"value\">00:05:00</span>");
    }

    [Fact]
    public void GameStats_ShouldUpdate_WhenMovesAreRecorded()
    {
        // Arrange
        var cut = RenderComponent<GameStats>();

        // Act
        _session.RecordMove(true);
        _session.RecordMove(false);

        // Assert
        cut.Find(".game-stats .stat-item:nth-child(1) .value").MarkupMatches("<span class=\"value\">2</span>");
        cut.Find(".game-stats .stat-item:nth-child(2) .value").MarkupMatches("<span class=\"value\">1</span>");
    }
}