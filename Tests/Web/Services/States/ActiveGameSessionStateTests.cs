using Sudoku.Web.Server.Services.States;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Web.Services.States;

public class ActiveGameSessionStateTests : GameSessionStateTests<ActiveGameSessionState>
{
    protected override ActiveGameSessionState ResolveSut()
    {
        return new ActiveGameSessionState(GameState, MockTimer.Object);
    }

    [Fact]
    public void RecordMove_WithValidMove_ShouldIncrementTotalMoves()
    {
        // Arrange
        var sut = ResolveSut();
        var initialTotalMoves = sut.TotalMoves;

        // Act
        sut.RecordMove(true);

        // Assert
        sut.TotalMoves.Should().Be(initialTotalMoves + 1);
        sut.InvalidMoves.Should().Be(0);
    }

    [Fact]
    public void RecordMove_WithInvalidMove_ShouldIncrementBothMoves()
    {
        // Arrange
        var sut = ResolveSut();
        var initialTotalMoves = sut.TotalMoves;
        var initialInvalidMoves = sut.InvalidMoves;

        // Act
        sut.RecordMove(false);

        // Assert
        sut.TotalMoves.Should().Be(initialTotalMoves + 1);
        sut.InvalidMoves.Should().Be(initialInvalidMoves + 1);
    }

    [Fact]
    public void RecordMove_ShouldRaiseOnMoveRecorded()
    {
        // Arrange
        var sut = ResolveSut();
        var eventRaised = false;
        sut.OnMoveRecorded += (sender, args) => eventRaised = true;

        // Act
        sut.RecordMove(true);

        // Assert
        eventRaised.Should().BeTrue();
    }

    [Fact]
    public void ReloadBoard_ShouldUpdateBoard()
    {
        // Arrange
        var sut = ResolveSut();
        var newBoard = new Cell[81];
        var newGameState = new GameStateMemory("new-puzzle", newBoard);

        // Act
        sut.ReloadBoard(newGameState);

        // Assert
        sut.Board.Should().BeEquivalentTo(newBoard);
    }

    [Fact]
    public void Start_ShouldNotStartTimer()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        sut.Start();

        // Assert
        MockTimer.VerifyStarted(Times.Never);
    }

    [Fact]
    public void Pause_ShouldPauseTimer()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        sut.Pause();

        // Assert
        MockTimer.VerifyPaused(Times.Once);
    }

    [Fact]
    public void Resume_ShouldResumeTimer()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        sut.Resume();

        // Assert
        MockTimer.VerifyResumed(Times.Once);
    }

    [Fact]
    public void End_ShouldPauseTimer()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        sut.End();

        // Assert
        MockTimer.VerifyPaused(Times.Once);
    }
}