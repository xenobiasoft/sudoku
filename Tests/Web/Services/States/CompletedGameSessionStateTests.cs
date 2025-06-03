using Sudoku.Web.Server.Services.States;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Web.Services.States;

public class CompletedGameSessionStateTests : GameSessionStateTests<CompletedGameSessionState>
{
    protected override CompletedGameSessionState ResolveSut()
    {
        return new CompletedGameSessionState(GameState, MockTimer.Object);
    }

    [Fact]
    public void RecordMove_ShouldNotIncrementMoves()
    {
        // Arrange
        var sut = ResolveSut();
        var initialTotalMoves = sut.TotalMoves;
        var initialInvalidMoves = sut.InvalidMoves;

        // Act
        sut.RecordMove(true);

        // Assert
        sut.TotalMoves.Should().Be(initialTotalMoves);
        sut.InvalidMoves.Should().Be(initialInvalidMoves);
    }

    [Fact]
    public void ReloadBoard_ShouldNotChangeBoard()
    {
        // Arrange
        var sut = ResolveSut();
        var initialBoard = sut.Board;
        var newGameState = new GameStateMemory("new-puzzle", new Cell[81]);

        // Act
        sut.ReloadBoard(newGameState);

        // Assert
        sut.Board.Should().BeEquivalentTo(initialBoard);
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
    public void Pause_ShouldNotPauseTimer()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        sut.Pause();

        // Assert
        MockTimer.VerifyPaused(Times.Never);
    }

    [Fact]
    public void Resume_ShouldNotResumeTimer()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        sut.Resume();

        // Assert
        MockTimer.VerifyResumed(Times.Never);
    }

    [Fact]
    public void End_ShouldNotPauseTimer()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        sut.End();

        // Assert
        MockTimer.VerifyPaused(Times.Never);
    }
}