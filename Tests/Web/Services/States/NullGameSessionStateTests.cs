using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.States;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Web.Services.States;

public class NullGameSessionStateTests : GameSessionStateTests<NullGameSessionState>
{
    protected override NullGameSessionState ResolveSut()
    {
        return NullGameSessionState.Instance;
    }

    [Fact]
    public void Instance_ShouldBeSingleton()
    {
        // Act
        var instance1 = NullGameSessionState.Instance;
        var instance2 = NullGameSessionState.Instance;

        // Assert
        instance1.Should().BeSameAs(instance2);
    }

    [Fact]
    public void InvalidMoves_ShouldReturnZero()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var result = sut.InvalidMoves;

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void TotalMoves_ShouldReturnZero()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var result = sut.TotalMoves;

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void PlayDuration_ShouldReturnZero()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var result = sut.PlayDuration;

        // Assert
        result.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void Timer_ShouldReturnNullGameTimer()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var result = sut.Timer;

        // Assert
        result.Should().BeOfType<NullGameTimer>();
    }

    [Fact]
    public void RecordMove_ShouldNotThrowException()
    {
        // Arrange
        var sut = ResolveSut();

        // Act & Assert
        sut.Invoking(x => x.RecordMove(true)).Should().NotThrow();
    }

    [Fact]
    public void ReloadBoard_ShouldNotThrowException()
    {
        // Arrange
        var sut = ResolveSut();
        var gameState = new GameStateMemory("test", []);

        // Act & Assert
        sut.Invoking(x => x.ReloadBoard(gameState)).Should().NotThrow();
    }

    [Fact]
    public void Start_ShouldNotThrowException()
    {
        // Arrange
        var sut = ResolveSut();

        // Act & Assert
        sut.Invoking(x => x.Start()).Should().NotThrow();
    }

    [Fact]
    public void Pause_ShouldNotThrowException()
    {
        // Arrange
        var sut = ResolveSut();

        // Act & Assert
        sut.Invoking(x => x.Pause()).Should().NotThrow();
    }

    [Fact]
    public void Resume_ShouldNotThrowException()
    {
        // Arrange
        var sut = ResolveSut();

        // Act & Assert
        sut.Invoking(x => x.Resume()).Should().NotThrow();
    }

    [Fact]
    public void End_ShouldNotThrowException()
    {
        // Arrange
        var sut = ResolveSut();

        // Act & Assert
        sut.Invoking(x => x.End()).Should().NotThrow();
    }

    public override void Alias_ShouldReturnGameStateAlias()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var result = sut.Alias;

        // Assert
        result.Should().BeEmpty();
    }

    public override void PuzzleId_ShouldReturnGameStatePuzzleId()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var result = sut.PuzzleId;

        // Assert
        result.Should().BeEmpty();
    }

    public override void Board_ShouldReturnGameStateBoard()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var result = sut.Board;

        // Assert
        result.Should().BeEmpty();
    }

    public override void Timer_ShouldReturnProvidedTimer()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var result = sut.Timer;

        // Assert
        result.Should().BeOfType<NullGameTimer>();
    }
}