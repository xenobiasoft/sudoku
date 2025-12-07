using DepenMock.XUnit;
using Sudoku.Domain.ValueObjects;
using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.HttpClients;
using UnitTests.Helpers.Factories;

namespace UnitTests.Web.Services;

public class GameManagerTests : BaseTestByAbstraction<GameManager, IGameManager>
{
    private const string Alias = "TestPlayer";
    private const string GameId = "TestGameId";

    private readonly Mock<IGameApiClient> _mockGameApiClient;
    private readonly Mock<ILocalStorageService> _mockLocalStorageService;

    public GameManagerTests()
    {
        _mockGameApiClient = Container.ResolveMock<IGameApiClient>();
        _mockLocalStorageService = Container.ResolveMock<ILocalStorageService>();
    }

	[Theory]
    [InlineData(nameof(GameStatus.NotStarted), nameof(GameStatus.InProgress))]
    [InlineData(nameof(GameStatus.InProgress), nameof(GameStatus.InProgress))]
    [InlineData(nameof(GameStatus.Paused), nameof(GameStatus.InProgress))]
    [InlineData(nameof(GameStatus.Abandoned), nameof(GameStatus.Abandoned))]
    [InlineData(nameof(GameStatus.Completed), nameof(GameStatus.Completed))]
    public async Task StartGameAsync_SetsGameStatusToExpected(string currentGameStatus, string expectedGameStatus)
	{
		// Arrange
        var game = GameModelFactory
            .Build()
            .WithDifficulty(GameDifficulty.Easy)
            .WithStatus(currentGameStatus)
            .Create();
        _mockGameApiClient.SetupGetGame(game);
        var sut = ResolveSut();
        await sut.LoadGameAsync(Alias, GameId);

		// Act
		await sut.StartGameAsync();

        // Assert
        sut.Game.Status.Should().Be(expectedGameStatus);
	}
}