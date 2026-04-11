using DepenMock.XUnit;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Factories;

namespace UnitTests.Domain;

public class SudokuGameMoveHistoryTests : BaseTestByType<SudokuGame>
{
    [Fact]
    public void GetMoveHistory_NewGame_ReturnsEmptyCollection()
    {
        // Arrange
        var sut = CreateGame();

        // Act
        var history = sut.MoveHistory;

        // Assert
        history.Should().BeEmpty();
    }

    [Fact]
    public void MakeMove_AddsEntryToMoveHistory()
    {
        // Arrange
        var sut = CreateGame();
        sut.StartGame();

        // Act
        sut.MakeMove(0, 0, 5);
        var history = sut.MoveHistory;

        // Assert
        history.Should().HaveCount(1);
        history[0].Row.Should().Be(0);
        history[0].Column.Should().Be(0);
        history[0].PreviousValue.Should().BeNull();
        history[0].NewValue.Should().Be(5);
    }

    [Fact]
    public void UndoLastMove_RemovesEntryFromMoveHistory()
    {
        // Arrange
        var sut = CreateGame();
        sut.StartGame();
        sut.MakeMove(0, 0, 5);

        // Act
        sut.UndoLastMove();
        var history = sut.MoveHistory;

        // Assert
        history.Should().BeEmpty();
    }

    [Fact]
    public void ResetGame_ClearsMoveHistory()
    {
        // Arrange
        var sut = CreateGame();
        sut.StartGame();
        sut.MakeMove(0, 0, 5);
        sut.MakeMove(1, 1, 3);

        // Act
        sut.ResetGame();
        var history = sut.MoveHistory;

        // Assert
        history.Should().BeEmpty();
    }

    [Fact]
    public void Reconstitute_WithMoveHistory_RestoresMoveHistory()
    {
        // Arrange
        var cells = CellsFactory.CreateEmptyCells();
        var moveHistory = new List<MoveHistory>
        {
            new(0, 0, null, 5),
            new(1, 1, null, 3)
        };

        // Act
        var sut = SudokuGame.Reconstitute(
            GameId.New(),
            PlayerAlias.Create("TestPlayer"),
            GameDifficulty.Easy,
            GameStatusEnum.InProgress,
            GameStatistics.Create(),
            cells,
            moveHistory,
            DateTime.UtcNow,
            DateTime.UtcNow,
            null,
            null
        );

        // Assert
        sut.MoveHistory.Should().HaveCount(2);
        sut.MoveHistory[0].NewValue.Should().Be(5);
        sut.MoveHistory[1].NewValue.Should().Be(3);
    }

    private static SudokuGame CreateGame()
    {
        var cells = CellsFactory.CreateEmptyCells();
        return SudokuGame.Create(
            PlayerAlias.Create("TestPlayer"),
            GameDifficulty.Easy,
            cells
        );
    }
}