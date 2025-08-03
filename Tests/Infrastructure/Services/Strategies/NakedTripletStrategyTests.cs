using DepenMock.XUnit;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Services.Strategies;

namespace UnitTests.Infrastructure.Services.Strategies;

public class NakedTripletStrategyTests : BaseTestByType<TripletStrategies>
{
    [Fact]
    public void HandleNakedTriplets_WithCellReducedToSingleValue_SetsCellValue()
    {
        // Arrange
        var cells = CreateNakedTripletWithSingleValueCells();

        // Act
        var result = TripletStrategies.HandleNakedTriplets(cells);

        // Assert
        result.Should().BeTrue();
        
        // Verify cell was set to its single possible value
        cells[3].Value.Should().Be(4);
        cells[3].PossibleValues.Should().BeEmpty();
    }

    [Fact]
    public void HandleNakedTriplets_WithNoNakedTriplets_ReturnsFalse()
    {
        // Arrange
        var cells = CreateCellsWithoutNakedTriplets();

        // Act
        var result = TripletStrategies.HandleNakedTriplets(cells);

        // Assert
        result.Should().BeFalse();
        
        // Verify cells remain unchanged
        cells[0].PossibleValues.Should().BeEquivalentTo([1, 2]);
        cells[1].PossibleValues.Should().BeEquivalentTo([3, 4]);
        cells[2].PossibleValues.Should().BeEquivalentTo([5, 6]);
    }

    [Fact]
    public void HandleNakedTriplets_WithEmptyColumnCells_ReturnsFalse()
    {
        // Arrange
        var cells = new List<Cell>();

        // Act
        var result = TripletStrategies.HandleNakedTriplets(cells);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HandleNakedTriplets_WithLessThanThreeCells_ReturnsFalse()
    {
        // Arrange
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0),
            Cell.CreateEmpty(1, 0)
        };
        
        cells[0].PossibleValues.UnionWith([1, 2, 3]);
        cells[1].PossibleValues.UnionWith([1, 2, 3]);

        // Act
        var result = TripletStrategies.HandleNakedTriplets(cells);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HandleNakedTriplets_WithCombinedValuesNotEqualToThree_SkipsCombination()
    {
        // Arrange
        var cells = CreateCellsWithMoreThanThreeValues();

        // Act
        var result = TripletStrategies.HandleNakedTriplets(cells);

        // Assert
        result.Should().BeFalse();
        
        // Verify cells remain unchanged since no valid triplet exists
        cells[0].PossibleValues.Should().BeEquivalentTo([1, 2, 3, 4]);
        cells[1].PossibleValues.Should().BeEquivalentTo([1, 2, 3, 5]);
        cells[2].PossibleValues.Should().BeEquivalentTo([1, 2, 3, 6]);
    }

    [Fact]
    public void HandleNakedTriplets_WithInvalidTripletNotAllValuesInCells_SkipsCombination()
    {
        // Arrange
        var cells = CreateInvalidTripletCells();

        // Act
        var result = TripletStrategies.HandleNakedTriplets(cells);

        // Assert
        result.Should().BeFalse();
        
        // Verify cells remain unchanged
        cells[0].PossibleValues.Should().BeEquivalentTo([1, 2]);
        cells[1].PossibleValues.Should().BeEquivalentTo([1, 3]);
        cells[2].PossibleValues.Should().BeEquivalentTo([2, 4]); // 4 is not in triplet values [1,2,3]
    }

    [Fact]
    public void HandleNakedTriplets_WithCellsNotSubsetOfTriplet_SkipsCombination()
    {
        // Arrange
        var cells = CreateCellsNotSubsetOfTriplet();

        // Act
        var result = TripletStrategies.HandleNakedTriplets(cells);

        // Assert
        result.Should().BeFalse();
        
        // Verify cells remain unchanged
        cells[0].PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        cells[1].PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        cells[2].PossibleValues.Should().BeEquivalentTo([1, 2, 3, 4]); // Contains value outside triplet
    }

    [Fact]
    public void HandleNakedTriplets_WhenRemovalLeadsToEmptyPossibleValues_ThrowsInvalidMoveException()
    {
        // Arrange
        var cells = CreateCellsLeadingToEmptyPossibleValues();

        // Act & Assert
        var action = () => TripletStrategies.HandleNakedTriplets(cells);
        action.Should().Throw<InvalidMoveException>()
            .WithMessage("Invalid move at row:3, col:0");
    }

    [Fact]
    public void HandleNakedTriplets_WithMultipleValidTriplets_ProcessesAll()
    {
        // Arrange
        var cells = CreateCellsWithMultipleTriplets();

        // Act
        var result = TripletStrategies.HandleNakedTriplets(cells);

        // Assert
        result.Should().BeTrue();
        
        // Verify both triplets are processed and values removed from other cells
        cells[6].Value.Should().Be(7); // Originally [1, 7], removed 1
        cells[7].Value.Should().Be(8); // Originally [4, 8], removed 4
    }

    [Fact]
    public void HandleNakedTriplets_WithPartialTripletMatch_DoesNotModifyCells()
    {
        // Arrange
        var cells = CreatePartialTripletCells();

        // Act
        var result = TripletStrategies.HandleNakedTriplets(cells);

        // Assert
        result.Should().BeFalse();
        
        // Verify cells remain unchanged since no complete triplet exists
        cells[0].PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        cells[1].PossibleValues.Should().BeEquivalentTo([1, 2]);
        cells[2].PossibleValues.Should().BeEquivalentTo([4, 5]);
    }

    [Fact]
    public void HandleNakedTriplets_WithExactThreeCellsTriplet_ProcessesTriplet()
    {
        // Arrange
        var cells = CreateExactTripletCells();

        // Act
        var result = TripletStrategies.HandleNakedTriplets(cells);

        // Assert
        result.Should().BeFalse(); // No other cells to remove values from
        
        // Verify triplet cells remain unchanged
        cells[0].PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        cells[1].PossibleValues.Should().BeEquivalentTo([1, 2]);
        cells[2].PossibleValues.Should().BeEquivalentTo([2, 3]);
    }

    [Fact]
    public void HandleNakedTriplets_WithMultipleChangesToSameCell_ReturnsTrueOnce()
    {
        // Arrange
        var cells = CreateCellsWithMultipleReductions();

        // Act
        var result = TripletStrategies.HandleNakedTriplets(cells);

        // Assert
        result.Should().BeTrue();
        
        // Verify cell that had multiple possible removals
        cells[3].Value.Should().Be(4); // 1 and 2 removed, leaving 4
    }

    [Fact]
    public void HandleNakedTriplets_WithValidNakedTriplet_ReturnsTrueAndRemovesFromOtherCells()
    {
        // Arrange
        var cells = CreateNakedTripletCells();

        // Act
        var result = TripletStrategies.HandleNakedTriplets(cells);

        // Assert
        result.Should().BeTrue();
        
        // Verify triplet cells maintain their values
        cells[0].PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        cells[1].PossibleValues.Should().BeEquivalentTo([1, 2]);
        cells[2].PossibleValues.Should().BeEquivalentTo([2, 3]);
        
        // Verify other cells have triplet values removed
        cells[3].Value.Should().Be(4); // Originally [1, 4], removed 1
        cells[4].Value.Should().Be(5); // Originally [2, 5], removed 2
        cells[5].Value.Should().Be(6); // Originally [3, 6], removed 3
    }

    #region Helper Methods
    private List<Cell> CreateNakedTripletCells()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0), // Triplet cell 1
            Cell.CreateEmpty(1, 0), // Triplet cell 2
            Cell.CreateEmpty(2, 0), // Triplet cell 3
            Cell.CreateEmpty(3, 0), // Other cell 1
            Cell.CreateEmpty(4, 0), // Other cell 2
            Cell.CreateEmpty(5, 0)  // Other cell 3
        };

        // Create naked triplet: [1,2,3] distributed across 3 cells
        cells[0].PossibleValues.UnionWith([1, 2, 3]);
        cells[1].PossibleValues.UnionWith([1, 2]);
        cells[2].PossibleValues.UnionWith([2, 3]);

        // Other cells that will have values removed
        cells[3].PossibleValues.UnionWith([1, 4]);
        cells[4].PossibleValues.UnionWith([2, 5]);
        cells[5].PossibleValues.UnionWith([3, 6]);

        return cells;
    }

    private List<Cell> CreateNakedTripletWithSingleValueCells()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0),
            Cell.CreateEmpty(1, 0),
            Cell.CreateEmpty(2, 0),
            Cell.CreateEmpty(3, 0)
        };

        // Naked triplet
        cells[0].PossibleValues.UnionWith([1, 2, 3]);
        cells[1].PossibleValues.UnionWith([1, 2]);
        cells[2].PossibleValues.UnionWith([2, 3]);

        // Cell that will be reduced to single value
        cells[3].PossibleValues.UnionWith([1, 4]); // Will become [4] after removal

        return cells;
    }

    private List<Cell> CreateCellsWithoutNakedTriplets()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0),
            Cell.CreateEmpty(1, 0),
            Cell.CreateEmpty(2, 0)
        };

        // No naked triplets - different values
        cells[0].PossibleValues.UnionWith([1, 2]);
        cells[1].PossibleValues.UnionWith([3, 4]);
        cells[2].PossibleValues.UnionWith([5, 6]);

        return cells;
    }

    private List<Cell> CreateCellsWithMoreThanThreeValues()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0),
            Cell.CreateEmpty(1, 0),
            Cell.CreateEmpty(2, 0)
        };

        // Combined values will be more than 3
        cells[0].PossibleValues.UnionWith([1, 2, 3, 4]);
        cells[1].PossibleValues.UnionWith([1, 2, 3, 5]);
        cells[2].PossibleValues.UnionWith([1, 2, 3, 6]);

        return cells;
    }

    private List<Cell> CreateInvalidTripletCells()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0),
            Cell.CreateEmpty(1, 0),
            Cell.CreateEmpty(2, 0)
        };

        // Not all triplet values appear in cells
        cells[0].PossibleValues.UnionWith([1, 2]);
        cells[1].PossibleValues.UnionWith([1, 3]);
        cells[2].PossibleValues.UnionWith([2, 4]); // 4 is not in triplet [1,2,3]

        return cells;
    }

    private List<Cell> CreateCellsNotSubsetOfTriplet()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0),
            Cell.CreateEmpty(1, 0),
            Cell.CreateEmpty(2, 0)
        };

        // One cell contains values outside the triplet
        cells[0].PossibleValues.UnionWith([1, 2, 3]);
        cells[1].PossibleValues.UnionWith([1, 2, 3]);
        cells[2].PossibleValues.UnionWith([1, 2, 3, 4]); // Contains 4 which is outside triplet

        return cells;
    }

    private List<Cell> CreateCellsLeadingToEmptyPossibleValues()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0),
            Cell.CreateEmpty(1, 0),
            Cell.CreateEmpty(2, 0),
            Cell.CreateEmpty(3, 0)
        };

        // Naked triplet
        cells[0].PossibleValues.UnionWith([1, 2, 3]);
        cells[1].PossibleValues.UnionWith([1, 2]);
        cells[2].PossibleValues.UnionWith([2, 3]);

        // Cell that will become empty after removal
        cells[3].PossibleValues.UnionWith([1, 2, 3]); // All values will be removed

        return cells;
    }

    private List<Cell> CreateCellsWithMultipleTriplets()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0), // First triplet
            Cell.CreateEmpty(1, 0),
            Cell.CreateEmpty(2, 0),
            Cell.CreateEmpty(3, 0), // Second triplet
            Cell.CreateEmpty(4, 0),
            Cell.CreateEmpty(5, 0),
            Cell.CreateEmpty(6, 0), // Other cell 1
            Cell.CreateEmpty(7, 0)  // Other cell 2
        };

        // First naked triplet [1,2,3]
        cells[0].PossibleValues.UnionWith([1, 2, 3]);
        cells[1].PossibleValues.UnionWith([1, 2]);
        cells[2].PossibleValues.UnionWith([2, 3]);

        // Second naked triplet [4,5,6]
        cells[3].PossibleValues.UnionWith([4, 5, 6]);
        cells[4].PossibleValues.UnionWith([4, 5]);
        cells[5].PossibleValues.UnionWith([5, 6]);

        // Other cells
        cells[6].PossibleValues.UnionWith([1, 7]);
        cells[7].PossibleValues.UnionWith([4, 8]);

        return cells;
    }

    private List<Cell> CreatePartialTripletCells()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0),
            Cell.CreateEmpty(1, 0),
            Cell.CreateEmpty(2, 0)
        };

        // Not a valid triplet pattern
        cells[0].PossibleValues.UnionWith([1, 2, 3]);
        cells[1].PossibleValues.UnionWith([1, 2]);
        cells[2].PossibleValues.UnionWith([4, 5]); // Different values, no triplet

        return cells;
    }

    private List<Cell> CreateExactTripletCells()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0),
            Cell.CreateEmpty(1, 0),
            Cell.CreateEmpty(2, 0)
        };

        // Perfect triplet with no other cells
        cells[0].PossibleValues.UnionWith([1, 2, 3]);
        cells[1].PossibleValues.UnionWith([1, 2]);
        cells[2].PossibleValues.UnionWith([2, 3]);

        return cells;
    }

    private List<Cell> CreateCellsWithMultipleReductions()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0),
            Cell.CreateEmpty(1, 0),
            Cell.CreateEmpty(2, 0),
            Cell.CreateEmpty(3, 0)
        };

        // Naked triplet
        cells[0].PossibleValues.UnionWith([1, 2, 3]);
        cells[1].PossibleValues.UnionWith([1, 2]);
        cells[2].PossibleValues.UnionWith([2, 3]);

        // Cell that will have multiple values removed
        cells[3].PossibleValues.UnionWith([1, 2, 4]); // Both 1 and 2 will be removed

        return cells;
    }
    #endregion
}