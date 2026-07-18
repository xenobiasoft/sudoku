using DepenMock.Attributes;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.Events;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Factories;

namespace UnitTests.Domain;

[LogOutput(LogOutputTiming.Always)]
public class SudokuGameHintTests : MoqBaseTestByType<SudokuGame>
{
    private static readonly SudokuPuzzle Solution = PuzzleFactory.GetSolvedPuzzle();

    [Fact]
    public void RevealHint_WithEmptyCells_FillsACellWithTheSolvedValue()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();

        // Act
        var (row, column, value) = sut.RevealHint(Solution);

        // Assert
        var cell = sut.GetCell(row, column);
        cell.Value.Should().Be(value);
        cell.Value.Should().Be(Solution.GetCell(row, column).Value);
    }

    [Fact]
    public void RevealHint_MarksTheRevealedCellAsHint()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();

        // Act
        var (row, column, _) = sut.RevealHint(Solution);

        // Assert
        sut.GetCell(row, column).IsHint.Should().BeTrue();
    }

    [Fact]
    public void RevealHint_IncrementsHintsUsed()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();

        // Act
        sut.RevealHint(Solution);

        // Assert
        sut.Statistics.HintsUsed.Should().Be(1);
        sut.Statistics.HintsRemainingFor(sut.Size).Should().Be(BoardSize.Nine.MaxHints - 1);
    }

    [Fact]
    public void RevealHint_RaisesHintRevealedEvent()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();

        // Act
        sut.RevealHint(Solution);

        // Assert
        sut.DomainEvents.Should().Contain(e => e is HintRevealedEvent);
    }

    [Fact]
    public void RevealHint_LocksTheRevealedCell_SoMakeMoveThrows()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();
        var (row, column, _) = sut.RevealHint(Solution);

        // Act
        Action act = () => sut.MakeMove(row, column, 1);

        // Assert
        act.Should().Throw<CellIsFixedException>();
    }

    [Fact]
    public void RevealHint_DoesNotAddToMoveHistory()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();

        // Act
        sut.RevealHint(Solution);

        // Assert
        sut.MoveHistory.Should().BeEmpty();
        Action undo = () => sut.UndoLastMove();
        undo.Should().Throw<NoMoveHistoryException>();
    }

    [Fact]
    public void RevealHint_WhenHintLimitReached_ThrowsHintLimitReachedException()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();
        for (var i = 0; i < BoardSize.Nine.MaxHints; i++)
        {
            sut.RevealHint(Solution);
        }

        // Act
        Action act = () => sut.RevealHint(Solution);

        // Assert
        act.Should().Throw<HintLimitReachedException>();
    }

    [Fact]
    public void RevealHint_WhenGameNotInProgress_ThrowsGameNotInProgressException()
    {
        // Arrange
        var sut = GameFactory.CreateGameWithCells(CellsFactory.CreateEmptyCells());

        // Act
        Action act = () => sut.RevealHint(Solution);

        // Assert
        act.Should().Throw<GameNotInProgressException>();
    }

    [Fact]
    public void RevealHint_WhenNoEmptyCells_ThrowsNoAvailableCellsForHintException()
    {
        // Arrange - all cells are fixed clues, leaving nothing to reveal
        var sut = GameFactory.CreateGameWithCells(CellsFactory.CreateCompletedCells());
        sut.StartGame();

        // Act
        Action act = () => sut.RevealHint(Solution);

        // Assert
        act.Should().Throw<NoAvailableCellsForHintException>();
    }

    [Fact]
    public void ResetGame_AfterHint_ClearsRevealedCellAndResetsHintsUsed()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();
        var (row, column, _) = sut.RevealHint(Solution);

        // Act
        sut.ResetGame();

        // Assert
        var cell = sut.GetCell(row, column);
        cell.IsHint.Should().BeFalse();
        cell.HasValue.Should().BeFalse();
        sut.Statistics.HintsUsed.Should().Be(0);
    }

    [Fact]
    public void RevealHint_CompletingTheBoard_CompletesTheGame()
    {
        // Arrange - a board with a single empty cell and a matching solution
        var completed = CellsFactory.CreateCompletedCells().ToList();
        var cells = completed
            .Select(c => c.Row == 0 && c.Column == 0 ? Cell.CreateEmpty(0, 0, BoardSize.Nine) : c)
            .ToList();
        var sut = GameFactory.CreateGameWithCells(cells);
        sut.StartGame();
        var solution = SudokuPuzzle.Create("solution", GameDifficulty.Easy, BoardSize.Nine, CellsFactory.CreateCompletedCells().ToList());

        // Act
        sut.RevealHint(solution);

        // Assert
        sut.Status.Should().Be(GameStatusEnum.Completed);
        sut.DomainEvents.Should().Contain(e => e is GameCompletedEvent);
    }

    private static SudokuGame CreateStartedEmptyGame()
    {
        var game = GameFactory.CreateGameWithCells(CellsFactory.CreateEmptyCells());
        game.StartGame();
        return game;
    }
}
