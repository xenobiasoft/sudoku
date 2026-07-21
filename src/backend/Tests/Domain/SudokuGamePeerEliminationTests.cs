using DepenMock.Attributes;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Factories;

namespace UnitTests.Domain;

[LogOutput(LogOutputTiming.Always)]
public class SudokuGamePeerEliminationTests : MoqBaseTestByType<SudokuGame>
{
    [Fact]
    public void MakeMove_WithValue_RemovesValueFromRowPeerPossibleValues()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();
        sut.AddPossibleValue(0, 5, 5);

        // Act
        sut.MakeMove(0, 0, 5);

        // Assert
        sut.GetCell(0, 5).PossibleValues.Should().NotContain(5);
    }

    [Fact]
    public void MakeMove_WithValue_RemovesValueFromColumnPeerPossibleValues()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();
        sut.AddPossibleValue(5, 0, 5);

        // Act
        sut.MakeMove(0, 0, 5);

        // Assert
        sut.GetCell(5, 0).PossibleValues.Should().NotContain(5);
    }

    [Fact]
    public void MakeMove_WithValue_RemovesValueFromBoxPeerPossibleValues()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();
        sut.AddPossibleValue(1, 1, 5);

        // Act
        sut.MakeMove(0, 0, 5);

        // Assert
        sut.GetCell(1, 1).PossibleValues.Should().NotContain(5);
    }

    [Fact]
    public void MakeMove_WithValue_DoesNotAffectUnrelatedCellPossibleValues()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();
        sut.AddPossibleValue(4, 4, 5);

        // Act
        sut.MakeMove(0, 0, 5);

        // Assert
        sut.GetCell(4, 4).PossibleValues.Should().Contain(5);
    }

    [Fact]
    public void MakeMove_WithValue_DoesNotThrow_WhenPeerCellAlreadyHasValue()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();
        sut.MakeMove(0, 5, 3);

        // Act
        var act = () => sut.MakeMove(0, 0, 5);

        // Assert
        act.Should().NotThrow();
        sut.GetCell(0, 5).Value.Should().Be(3);
    }

    [Fact]
    public void MakeMove_ClearingCell_DoesNotEliminatePeerCandidates()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();
        sut.MakeMove(0, 0, 5);
        sut.AddPossibleValue(0, 5, 5);

        // Act
        sut.MakeMove(0, 0, null);

        // Assert
        sut.GetCell(0, 5).PossibleValues.Should().Contain(5);
    }

    [Fact]
    public void MakeMove_RecordsPeerEliminationsInHistoryEntry()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();
        sut.AddPossibleValue(0, 5, 5);
        sut.AddPossibleValue(5, 0, 5);
        sut.AddPossibleValue(1, 1, 5);

        // Act
        sut.MakeMove(0, 0, 5);

        // Assert
        var moveEntry = sut.GetHistory().Last().Should().BeOfType<MoveHistoryEntry>().Subject;
        moveEntry.PeerEliminations.Should().HaveCount(3);
        moveEntry.PeerEliminations.Should().Contain(p => p.Row == 0 && p.Column == 5 && p.Value == 5);
        moveEntry.PeerEliminations.Should().Contain(p => p.Row == 5 && p.Column == 0 && p.Value == 5);
        moveEntry.PeerEliminations.Should().Contain(p => p.Row == 1 && p.Column == 1 && p.Value == 5);
    }

    [Fact]
    public void RevealHint_RemovesValueFromRowPeerPossibleValues()
    {
        // Arrange - leave row 0 entirely empty/unlocked so the hint target is
        // guaranteed to land there, and seed every digit as a candidate on every
        // row-0 cell so whichever column gets picked, its row peers still have
        // the revealed value to eliminate.
        var sut = CreateGameWithEmptyRowZero();
        for (var col = 0; col < 9; col++)
        {
            for (var digit = 1; digit <= 9; digit++)
            {
                sut.AddPossibleValue(0, col, digit);
            }
        }

        // Act
        var (row, column, value) = sut.RevealHint(PuzzleFactory.GetSolvedPuzzle());

        // Assert
        for (var col = 0; col < 9; col++)
        {
            if (col == column)
            {
                continue;
            }

            sut.GetCell(row, col).PossibleValues.Should().NotContain(value);
        }
    }

    [Fact]
    public void RevealHint_DoesNotPushEliminationsToHistory()
    {
        // Arrange
        var sut = CreateGameWithEmptyRowZero();
        for (var col = 0; col < 9; col++)
        {
            sut.AddPossibleValue(0, col, 1);
        }

        var historyCountBeforeHint = sut.GetHistory().Count;

        // Act
        sut.RevealHint(PuzzleFactory.GetSolvedPuzzle());

        // Assert
        sut.GetHistory().Should().HaveCount(historyCountBeforeHint);
        sut.GetHistory().Should().NotContain(e => e is MoveHistoryEntry);
    }

    [Fact]
    public void UndoLastMove_RestoresCascadedPeerEliminationsAtomically()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();
        sut.AddPossibleValue(0, 5, 5);
        sut.AddPossibleValue(5, 0, 5);
        sut.AddPossibleValue(1, 1, 5);
        sut.MakeMove(0, 0, 5);

        // Act
        sut.UndoLastMove();

        // Assert
        sut.GetCell(0, 0).HasValue.Should().BeFalse();
        sut.GetCell(0, 5).PossibleValues.Should().Contain(5);
        sut.GetCell(5, 0).PossibleValues.Should().Contain(5);
        sut.GetCell(1, 1).PossibleValues.Should().Contain(5);
    }

    [Fact]
    public void UndoLastMove_WhenLastEntryIsPencilMarkEdit_DoesNotChangeStatistics()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();
        sut.MakeMove(0, 0, 5);
        sut.AddPossibleValue(1, 1, 3);
        var totalMovesBeforeUndo = sut.Statistics.TotalMoves;
        var validMovesBeforeUndo = sut.Statistics.ValidMoves;

        // Act
        sut.UndoLastMove();

        // Assert
        sut.Statistics.TotalMoves.Should().Be(totalMovesBeforeUndo);
        sut.Statistics.ValidMoves.Should().Be(validMovesBeforeUndo);
        sut.GetCell(1, 1).PossibleValues.Should().NotContain(3);
    }

    [Fact]
    public void UndoLastMove_UndoingManualAdd_RemovesThePossibleValue()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();
        sut.AddPossibleValue(2, 2, 7);

        // Act
        sut.UndoLastMove();

        // Assert
        sut.GetCell(2, 2).PossibleValues.Should().NotContain(7);
    }

    [Fact]
    public void UndoLastMove_UndoingManualRemove_RestoresThePossibleValue()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();
        sut.AddPossibleValue(2, 2, 7);
        sut.RemovePossibleValue(2, 2, 7);

        // Act
        sut.UndoLastMove();

        // Assert
        sut.GetCell(2, 2).PossibleValues.Should().Contain(7);
    }

    [Fact]
    public void UndoLastMove_UndoingClear_RestoresAllPreviousPossibleValues()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();
        sut.AddPossibleValue(2, 2, 7);
        sut.AddPossibleValue(2, 2, 8);
        sut.ClearPossibleValues(2, 2);

        // Act
        sut.UndoLastMove();

        // Assert
        sut.GetCell(2, 2).PossibleValues.Should().Contain(7);
        sut.GetCell(2, 2).PossibleValues.Should().Contain(8);
    }

    [Fact]
    public void AddPossibleValue_PushesEntryToHistory()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();

        // Act
        sut.AddPossibleValue(3, 3, 4);

        // Assert
        var entry = sut.GetHistory().Last().Should().BeOfType<PossibleValueAddedEntry>().Subject;
        entry.Row.Should().Be(3);
        entry.Column.Should().Be(3);
        entry.Value.Should().Be(4);
    }

    [Fact]
    public void RemovePossibleValue_PushesEntryToHistory()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();
        sut.AddPossibleValue(3, 3, 4);

        // Act
        sut.RemovePossibleValue(3, 3, 4);

        // Assert
        var entry = sut.GetHistory().Last().Should().BeOfType<PossibleValueRemovedEntry>().Subject;
        entry.Row.Should().Be(3);
        entry.Column.Should().Be(3);
        entry.Value.Should().Be(4);
    }

    [Fact]
    public void ClearPossibleValues_PushesEntryWithPreviousValuesToHistory()
    {
        // Arrange
        var sut = CreateStartedEmptyGame();
        sut.AddPossibleValue(3, 3, 4);
        sut.AddPossibleValue(3, 3, 6);

        // Act
        sut.ClearPossibleValues(3, 3);

        // Assert
        var entry = sut.GetHistory().Last().Should().BeOfType<PossibleValuesClearedEntry>().Subject;
        entry.PreviousValues.Should().BeEquivalentTo([4, 6]);
    }

    private static SudokuGame CreateStartedEmptyGame()
    {
        var game = GameFactory.CreateGameWithCells(CellsFactory.CreateEmptyCells());
        game.StartGame();
        return game;
    }

    private static SudokuGame CreateGameWithEmptyRowZero()
    {
        var cells = CellsFactory.CreateCompletedCells()
            .Select(c => c.Row == 0
                ? Cell.CreateEmpty(c.Row, c.Column, BoardSize.Nine)
                : Cell.CreateFixed(c.Row, c.Column, c.Value!.Value, BoardSize.Nine))
            .ToList();

        var game = GameFactory.CreateGameWithCells(cells);
        game.StartGame();
        return game;
    }
}
