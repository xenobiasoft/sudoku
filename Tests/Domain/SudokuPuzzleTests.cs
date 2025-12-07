using DepenMock.XUnit;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Domain;

public class SudokuPuzzleTests : BaseTestByType<SudokuPuzzle>
{
    [Fact]
    public void Clone_CreatesDeepCopyOfPuzzle()
    {
        // Arrange
        var cells = CreateTestCells();
        var originalPuzzle = SudokuPuzzle.Create("test-puzzle-123", GameDifficulty.Medium, cells);

        // Act
        var clonedPuzzle = originalPuzzle.Clone() as SudokuPuzzle;

        // Assert
        clonedPuzzle.Should().NotBeSameAs(originalPuzzle);
        clonedPuzzle.PuzzleId.Should().Be(originalPuzzle.PuzzleId);
        clonedPuzzle.Difficulty.Should().Be(originalPuzzle.Difficulty);
        clonedPuzzle.Cells.Should().NotBeSameAs(originalPuzzle.Cells);
        clonedPuzzle.Cells.Count.Should().Be(originalPuzzle.Cells.Count);
    }

    [Fact]
    public void Clone_EachCellIsDeepCopied()
    {
        // Arrange
        var cells = CreateTestCells();
        var originalPuzzle = SudokuPuzzle.Create("test-puzzle-456", GameDifficulty.Hard, cells);
        originalPuzzle.PopulatePossibleValues();

        // Act
        var clonedPuzzle = originalPuzzle.Clone() as SudokuPuzzle;

        // Assert
        for (int i = 0; i < originalPuzzle.Cells.Count; i++)
        {
            var originalCell = originalPuzzle.Cells[i];
            var clonedCell = clonedPuzzle.Cells[i];

            clonedCell.Should().NotBeSameAs(originalCell);
            clonedCell.Row.Should().Be(originalCell.Row);
            clonedCell.Column.Should().Be(originalCell.Column);
            clonedCell.Value.Should().Be(originalCell.Value);
            clonedCell.IsFixed.Should().Be(originalCell.IsFixed);
            clonedCell.PossibleValues.Should().NotBeSameAs(originalCell.PossibleValues);
            clonedCell.PossibleValues.Should().BeEquivalentTo(originalCell.PossibleValues);
        }
    }

    [Fact]
    public void Clone_ModifyingClonedPuzzle_DoesNotAffectOriginal()
    {
        // Arrange
        var cells = CreateTestCells();
        var originalPuzzle = SudokuPuzzle.Create("test-puzzle-789", GameDifficulty.Easy, cells);
        var clonedPuzzle = originalPuzzle.Clone() as SudokuPuzzle;

        // Act
        var cellToModify = clonedPuzzle.GetCell(1, 1);
        cellToModify.SetValue(9);

        // Assert
        var originalCell = originalPuzzle.GetCell(1, 1);
        originalCell.Value.Should().BeNull();
        cellToModify.Value.Should().Be(9);
    }

    [Fact]
    public void Clone_WithPossibleValues_PreservesPossibleValues()
    {
        // Arrange
        var cells = CreateTestCells();
        var originalPuzzle = SudokuPuzzle.Create("test-puzzle-abc", GameDifficulty.Expert, cells);
        
        // Add some possible values to an empty cell
        var emptyCell = originalPuzzle.GetCell(1, 1);
        emptyCell.AddPossibleValue(3);
        emptyCell.AddPossibleValue(7);
        emptyCell.AddPossibleValue(9);

        // Act
        var clonedPuzzle = originalPuzzle.Clone() as SudokuPuzzle;

        // Assert
        var clonedCell = clonedPuzzle.GetCell(1, 1);
        clonedCell.PossibleValues.Should().NotBeSameAs(emptyCell.PossibleValues);
        clonedCell.PossibleValues.Should().BeEquivalentTo(new[] { 3, 7, 9 });

        // Modifying cloned cell should not affect original
        clonedCell.AddPossibleValue(2);
        emptyCell.PossibleValues.Should().BeEquivalentTo(new[] { 3, 7, 9 });
        clonedCell.PossibleValues.Should().BeEquivalentTo(new[] { 2, 3, 7, 9 });
    }

    [Fact]
    public void Clone_PreservesValidationRules()
    {
        // Arrange
        var cells = CreateTestCells();
        var originalPuzzle = SudokuPuzzle.Create("test-puzzle-def", GameDifficulty.Medium, cells);

        // Act
        var clonedPuzzle = originalPuzzle.Clone() as SudokuPuzzle;

        // Assert
        clonedPuzzle.IsValid().Should().BeTrue();
        clonedPuzzle.GetFixedCellCount().Should().Be(originalPuzzle.GetFixedCellCount());
        clonedPuzzle.GetEmptyCellCount().Should().Be(originalPuzzle.GetEmptyCellCount());
    }

    private static List<Cell> CreateTestCells()
    {
        var cells = new List<Cell>();
        
        // Create a 9x9 grid of cells
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (row == 0 && col == 0)
                {
                    // Add a fixed cell
                    cells.Add(Cell.CreateFixed(row, col, 5));
                }
                else if (row == 2 && col == 3)
                {
                    // Add another fixed cell
                    cells.Add(Cell.CreateFixed(row, col, 8));
                }
                else
                {
                    // Add empty cells
                    cells.Add(Cell.CreateEmpty(row, col));
                }
            }
        }
        
        return cells;
    }
}