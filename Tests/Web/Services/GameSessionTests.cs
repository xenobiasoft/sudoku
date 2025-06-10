using DepenMock.XUnit;
using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Web.Services;

public class GameSessionTests : BaseTestByAbstraction<GameSession, IGameSession>
{
    private readonly Mock<IGameSessionState> _mockSessionState;

    public GameSessionTests()
    {
        _mockSessionState = Container.ResolveMock<IGameSessionState>();
    }

    [Fact]
    public void Resume_ShouldResumeTimerWithSavedPlayDuration()
    {
        // Arrange
        var gameState = new GameStateMemory
        {
            PlayDuration = TimeSpan.FromMinutes(10),
            TotalMoves = 5,
            InvalidMoves = 1
        };
        var sut = ResolveSut();
        sut.ReloadBoard(gameState);
        sut.Start();
        sut.Pause();

        // Act
        sut.Resume();

        // Assert
        _mockSessionState.Verify(x => x.Resume(gameState.PlayDuration), Times.Once);
    }

    [Fact]
    public void ReloadBoard_ShouldPreservePlayDuration()
    {
        // Arrange
        var newGameState = new GameStateMemory
        {
            PlayDuration = TimeSpan.FromMinutes(15),
            TotalMoves = 6,
            InvalidMoves = 2
        };
        var sut = ResolveSut();

        // Act
        sut.ReloadBoard(newGameState);

        // Assert
        sut.GameState.PlayDuration.Should().Be(newGameState.PlayDuration);
    }

    [Fact]
    public void Resume_AfterUndo_ShouldResumeWithCorrectDuration()
    {
        // Arrange
        var sut = ResolveSut();
        sut.Start();
        sut.Pause();
        var newGameState = new GameStateMemory
        {
            PlayDuration = TimeSpan.FromMinutes(8),
            TotalMoves = 4,
            InvalidMoves = 1
        };
        sut.ReloadBoard(newGameState);

        // Act
        sut.Resume();

        // Assert
        _mockSessionState.Verify(x => x.Resume(newGameState.PlayDuration), Times.Once);
    }

    [Fact]
    public void Start_ShouldInitializeTimerWithZeroDuration()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        sut.Start();

        // Assert
        sut.GameState.PlayDuration.Should().BeCloseTo(TimeSpan.Zero, TimeSpan.FromMilliseconds(50));
    }
}