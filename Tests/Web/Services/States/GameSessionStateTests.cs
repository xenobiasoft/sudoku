using DepenMock.XUnit;
using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.States;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Web.Services.States;

public abstract class GameSessionStateTests<TState> : BaseTestByAbstraction<TState, IGameSessionState>
    where TState : class, IGameSessionState
{
    protected readonly Mock<IGameTimer> MockTimer;
    protected readonly GameStateMemory GameState;

    protected GameSessionStateTests()
    {
        MockTimer = Container.ResolveMock<IGameTimer>();
        GameState = new GameStateMemory("test-puzzle", [])
        {
            Alias = "test-alias",
            InvalidMoves = 0,
            TotalMoves = 0,
            PlayDuration = TimeSpan.Zero
        };
    }

    protected virtual TState ResolveSut()
    {
        return Container.Resolve<TState>();
    }

    [Fact]
    public virtual void Alias_ShouldReturnGameStateAlias()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var result = sut.Alias;

        // Assert
        result.Should().Be(GameState.Alias);
    }

    [Fact]
    public virtual void PuzzleId_ShouldReturnGameStatePuzzleId()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var result = sut.PuzzleId;

        // Assert
        result.Should().Be(GameState.PuzzleId);
    }

    [Fact]
    public virtual void Board_ShouldReturnGameStateBoard()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var result = sut.Board;

        // Assert
        result.Should().BeEquivalentTo(GameState.Board);
    }

    [Fact]
    public virtual void Timer_ShouldReturnProvidedTimer()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var result = sut.Timer;

        // Assert
        result.Should().Be(MockTimer.Object);
    }
}