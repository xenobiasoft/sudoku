using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.Services;

namespace UnitTests.Web.Components;

public class GameTimerTests : TestContext
{
    private Mock<IGameNotificationService> _mockGameNotificationService = new();

    public GameTimerTests()
    {
        Services.AddSingleton(_mockGameNotificationService.Object);
    }

	[Fact]
	public void GameTimer_RendersCorrectly()
	{
		// Arrange

		// Act
        var timer = RenderComponent<GameTimer>();

		// Assert
		timer.MarkupMatches("<div class=\"timer\"><label>0:00:00</label></div>");
	}

	[Fact]
	public async Task GameTimer_WhenStartingNewGame_StartsTimer()
	{
		// Arrange
        var timer = RenderComponent<GameTimer>();
        _mockGameNotificationService
            .Setup(x => x.NotifyGameStarted())
            .Raises(x => x.GameStarted += null);

        // Act
        await timer.InvokeAsync(() => _mockGameNotificationService.Object.NotifyGameStarted());

        // Assert
        timer.WaitForState(() => timer.Find("label").TextContent == "0:00:01");
        timer.Find("label").MarkupMatches("<label>0:00:01</label>");
    }
}