using DepenMock.XUnit;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.Events;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Domain;

public class SudokuGamePossibleValuesTests : BaseTestByType<SudokuGame>
{
    private readonly PlayerAlias _playerAlias = PlayerAlias.Create("TestPlayer");
    private readonly GameDifficulty _difficulty = GameDifficulty.Easy;
    private readonly List<Cell> _initialCells;

    public SudokuGamePossibleValuesTests()
    {
        _initialCells = CreateInitialCells();
    }

    [Fact]
    public void AddPossibleValue_ToEmptyCell_AddsPossibleValue()
    {
        // Arrange
        var game = SudokuGame.Create(_playerAlias, _difficulty, _initialCells);
        game.StartGame();
        int row = 2, col = 2, value = 3;

        // Act
        game.AddPossibleValue(row, col, value);

        // Assert
        var cell = game.GetCell(row, col);
        cell.PossibleValues.Should().ContainSingle(v => v == value);
        
        // Verify domain event was raised
        game.DomainEvents.Should().ContainSingle(e => e is PossibleValueAddedEvent);
        var @event = game.DomainEvents.OfType<PossibleValueAddedEvent>().First();
        @event.GameId.Should().Be(game.Id);
        @event.Row.Should().Be(row);
        @event.Column.Should().Be(col);
        @event.Value.Should().Be(value);
    }

    [Fact]
    public void AddPossibleValue_ToFixedCell_ThrowsCellIsFixedException()
    {
        // Arrange
        var game = SudokuGame.Create(_playerAlias, _difficulty, _initialCells);
        game.StartGame();
        int row = 0, col = 0, value = 3; // Cell at (0,0) is fixed

        // Act
        Action act = () => game.AddPossibleValue(row, col, value);

        // Assert
        act.Should().Throw<CellIsFixedException>()
            .WithMessage($"*Cannot modify fixed cell at position ({row}, {col})*");
    }

    [Fact]
    public void AddPossibleValue_ToCellWithValue_ThrowsInvalidOperationException()
    {
        // Arrange
        var game = SudokuGame.Create(_playerAlias, _difficulty, _initialCells);
        game.StartGame();
        int row = 1, col = 1, value = 3; // Cell at (1,1) has a value

        // Act
        Action act = () => game.AddPossibleValue(row, col, value);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"*Cannot add possible values to cell with a definite value*");
    }

    [Fact]
    public void AddPossibleValue_WhenGameNotInProgress_ThrowsGameNotInProgressException()
    {
        // Arrange
        var game = SudokuGame.Create(_playerAlias, _difficulty, _initialCells);
        // Don't start the game
        int row = 2, col = 2, value = 3;

        // Act
        Action act = () => game.AddPossibleValue(row, col, value);

        // Assert
        act.Should().Throw<GameNotInProgressException>()
            .WithMessage($"*Cannot add possible value in {GameStatusEnum.NotStarted} state*");
    }

    [Fact]
    public void ClearPossibleValues_FromCellWithPossibleValues_ClearsPossibleValues()
    {
        // Arrange
        var game = SudokuGame.Create(_playerAlias, _difficulty, _initialCells);
        game.StartGame();
        int row = 2, col = 2;
        game.AddPossibleValue(row, col, 3);
        game.AddPossibleValue(row, col, 5);
        game.AddPossibleValue(row, col, 7);
        game.ClearDomainEvents();

        // Act
        game.ClearPossibleValues(row, col);

        // Assert
        var cell = game.GetCell(row, col);
        cell.PossibleValues.Should().BeEmpty();

        // Verify domain event was raised
        game.DomainEvents.Should().ContainSingle(e => e is PossibleValuesClearedEvent);
        var @event = game.DomainEvents.OfType<PossibleValuesClearedEvent>().First();
        @event.GameId.Should().Be(game.Id);
        @event.Row.Should().Be(row);
        @event.Column.Should().Be(col);
    }

    [Fact]
    public void ClearPossibleValues_FromFixedCell_ThrowsCellIsFixedException()
    {
        // Arrange
        var game = SudokuGame.Create(_playerAlias, _difficulty, _initialCells);
        game.StartGame();
        int row = 0, col = 0; // Cell at (0,0) is fixed

        // Act
        Action act = () => game.ClearPossibleValues(row, col);

        // Assert
        act.Should().Throw<CellIsFixedException>()
            .WithMessage($"*Cannot modify fixed cell at position ({row}, {col})*");
    }

    [Fact]
    public void ClearPossibleValues_WhenGameNotInProgress_ThrowsGameNotInProgressException()
    {
        // Arrange
        var game = SudokuGame.Create(_playerAlias, _difficulty, _initialCells);
        // Don't start the game
        int row = 2, col = 2;

        // Act
        Action act = () => game.ClearPossibleValues(row, col);

        // Assert
        act.Should().Throw<GameNotInProgressException>()
            .WithMessage($"*Cannot clear possible values in {GameStatusEnum.NotStarted} state*");
    }

    [Fact]
    public void MakeMove_ClearsPossibleValues()
    {
        // Arrange
        var game = SudokuGame.Create(_playerAlias, _difficulty, _initialCells);
        game.StartGame();
        int row = 2, col = 2;
        game.AddPossibleValue(row, col, 3);
        game.AddPossibleValue(row, col, 5);

        // Act
        game.MakeMove(row, col, 3);

        // Assert
        var cell = game.GetCell(row, col);
        cell.Value.Should().Be(3);
        cell.PossibleValues.Should().BeEmpty();
    }

    [Fact]
    public void RemovePossibleValue_FromCellWithPossibleValue_RemovesPossibleValue()
    {
        // Arrange
        var game = SudokuGame.Create(_playerAlias, _difficulty, _initialCells);
        game.StartGame();
        int row = 2, col = 2, value = 3;
        game.AddPossibleValue(row, col, value);
        game.ClearDomainEvents();

        // Act
        game.RemovePossibleValue(row, col, value);

        // Assert
        var cell = game.GetCell(row, col);
        cell.PossibleValues.Should().BeEmpty();

        // Verify domain event was raised
        game.DomainEvents.Should().ContainSingle(e => e is PossibleValueRemovedEvent);
        var @event = game.DomainEvents.OfType<PossibleValueRemovedEvent>().First();
        @event.GameId.Should().Be(game.Id);
        @event.Row.Should().Be(row);
        @event.Column.Should().Be(col);
        @event.Value.Should().Be(value);
    }

    [Fact]
    public void RemovePossibleValue_FromFixedCell_ThrowsCellIsFixedException()
    {
        // Arrange
        var game = SudokuGame.Create(_playerAlias, _difficulty, _initialCells);
        game.StartGame();
        int row = 0, col = 0, value = 3; // Cell at (0,0) is fixed

        // Act
        Action act = () => game.RemovePossibleValue(row, col, value);

        // Assert
        act.Should().Throw<CellIsFixedException>()
            .WithMessage($"*Cannot modify fixed cell at position ({row}, {col})*");
    }

    [Fact]
    public void RemovePossibleValue_WhenGameNotInProgress_ThrowsGameNotInProgressException()
    {
        // Arrange
        var game = SudokuGame.Create(_playerAlias, _difficulty, _initialCells);
        // Don't start the game
        int row = 2, col = 2, value = 3;

        // Act
        Action act = () => game.RemovePossibleValue(row, col, value);

        // Assert
        act.Should().Throw<GameNotInProgressException>()
            .WithMessage($"*Cannot remove possible value in {GameStatusEnum.NotStarted} state*");
    }

    [Fact]
    public void ResetGame_ClearsPossibleValuesForAllNonFixedCells()
    {
        // Arrange
        var game = SudokuGame.Create(_playerAlias, _difficulty, _initialCells);
        game.StartGame();
        game.AddPossibleValue(2, 2, 3);
        game.AddPossibleValue(2, 2, 5);
        game.AddPossibleValue(3, 3, 7);

        // Act
        game.ResetGame();

        // Assert
        var cell1 = game.GetCell(2, 2);
        var cell2 = game.GetCell(3, 3);
        cell1.PossibleValues.Should().BeEmpty();
        cell2.PossibleValues.Should().BeEmpty();
    }

    private List<Cell> CreateInitialCells()
    {
        var cells = new List<Cell>();
        for (var row = 0; row < 9; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                if (row == 0 && col == 0)
                {
                    cells.Add(Cell.CreateFixed(row, col, 5));
                }
                else if (row == 1 && col == 1)
                {
                    cells.Add(Cell.Create(row, col, 7));
                }
                else
                {
                    cells.Add(Cell.CreateEmpty(row, col));
                }
            }
        }
        return cells;
    }
}