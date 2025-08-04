using DepenMock.XUnit;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Services.Strategies;

namespace UnitTests.Infrastructure.Services.Strategies;

public class HiddenTripletStrategyTests : BaseTestByType<TripletStrategies>
{
    [Fact]
    public void HandleHiddenTriplets_WithAllNumberCombinations_ProcessesCorrectly()
    {
        // Arrange
        var cells = CreateCellsForAllNumberCombinations();

        // Act
        var result = TripletStrategies.HandleHiddenTriplets(cells);

        // Assert
        result.Should().BeTrue();
        
        cells[0].Value.Should().Be(7);
        cells[1].Value.Should().Be(8);
        cells[2].Value.Should().Be(9);
    }

    [Fact]
    public void HandleHiddenTriplets_WithNoCandidateReduction_ReturnsFalse()
    {
        // Arrange
        var cells = CreateHiddenTripletWithoutReduction();

        // Act
        var result = TripletStrategies.HandleHiddenTriplets(cells);

        // Assert
        result.Should().BeFalse();
        
        // Verify cells remain unchanged since they already only contain triplet values
        cells[0].PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        cells[1].PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
        cells[2].PossibleValues.Should().BeEquivalentTo([1, 2, 3]);
    }

    [Fact]
    public void HandleHiddenTriplets_WithValidHiddenTriplet_ReturnsTrueAndRemovesOtherCandidates()
    {
        // Arrange
        var cells = CreateHiddenTripletCells();

        // Act
        var result = TripletStrategies.HandleHiddenTriplets(cells);

        // Assert
        result.Should().BeTrue();
        
        // Verify triplet cells were set to their individual values since they got reduced to single candidates
        cells[0].Value.Should().Be(1);
        cells[0].PossibleValues.Should().BeEmpty();
        cells[1].Value.Should().Be(2);
        cells[1].PossibleValues.Should().BeEmpty();
        cells[2].Value.Should().Be(3);
        cells[2].PossibleValues.Should().BeEmpty();
        
        // Verify other cells remain unchanged
        cells[3].PossibleValues.Should().BeEquivalentTo([4, 5, 6]);
    }

    [Fact]
    public void HandleHiddenTriplets_WithCellReducedToSingleValue_SetsCellValue()
    {
        // Arrange
        var cells = CreateHiddenTripletWithSingleValueCell();

        // Act
        var result = TripletStrategies.HandleHiddenTriplets(cells);

        // Assert
        result.Should().BeTrue();
        
        // Verify cell was set to its single possible value
        cells[0].Value.Should().Be(1);
        cells[0].PossibleValues.Should().BeEmpty();
    }

    [Fact]
    public void HandleHiddenTriplets_WithNoHiddenTriplets_ReturnsFalse()
    {
        // Arrange
        var cells = CreateCellsWithoutHiddenTriplets();

        // Act
        var result = TripletStrategies.HandleHiddenTriplets(cells);

        // Assert
        result.Should().BeFalse();
        
        // Verify cells remain unchanged
        cells[0].PossibleValues.Should().BeEquivalentTo([1, 2, 4, 5]);
        cells[1].PossibleValues.Should().BeEquivalentTo([1, 3, 6, 7]);
        cells[2].PossibleValues.Should().BeEquivalentTo([2, 3, 8, 9]);
        cells[3].PossibleValues.Should().BeEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9]);
    }

    [Fact]
    public void HandleHiddenTriplets_WithEmptyColumnCells_ReturnsFalse()
    {
        // Arrange
        var cells = new List<Cell>();

        // Act
        var result = TripletStrategies.HandleHiddenTriplets(cells);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HandleHiddenTriplets_WithTripletNumbersInMoreThanThreeCells_SkipsCombination()
    {
        // Arrange
        var cells = CreateTripletNumbersInMoreThanThreeCells();

        // Act
        var result = TripletStrategies.HandleHiddenTriplets(cells);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HandleHiddenTriplets_WithTripletNumbersInLessThanThreeCells_SkipsCombination()
    {
        // Arrange
        var cells = CreateTripletNumbersInLessThanThreeCells();

        // Act
        var result = TripletStrategies.HandleHiddenTriplets(cells);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HandleHiddenTriplets_WithMultipleValidHiddenTriplets_ProcessesAll()
    {
        // Arrange
        var cells = CreateCellsWithMultipleHiddenTriplets();

        // Act
        var result = TripletStrategies.HandleHiddenTriplets(cells);

        // Assert
        result.Should().BeTrue();
        
        cells[0].Value.Should().Be(1);
        cells[1].Value.Should().Be(2);
        cells[2].Value.Should().Be(3);
        cells[3].Value.Should().Be(4);
        cells[4].Value.Should().Be(5);
        cells[5].Value.Should().Be(6);
    }

    [Fact]
    public void HandleHiddenTriplets_WithOverlappingPossibleValues_OnlyRemovesNonTripletValues()
    {
        // Arrange
        var cells = CreateHiddenTripletWithOverlappingValues();

        // Act
        var result = TripletStrategies.HandleHiddenTriplets(cells);

        // Assert
        result.Should().BeTrue();
        
        // Verify only non-triplet values [4,5,6] are removed, triplet values [1,2,3] remain
        cells[0].PossibleValues.Should().BeEquivalentTo([1, 2]);
        cells[1].PossibleValues.Should().BeEquivalentTo([1, 3]);
        cells[2].PossibleValues.Should().BeEquivalentTo([2, 3]);
    }

    [Fact]
    public void HandleHiddenTriplets_WithPartialTripletMatch_DoesNotModifyCells()
    {
        // Arrange
        var cells = CreatePartialTripletCells();

        // Act
        var result = TripletStrategies.HandleHiddenTriplets(cells);

        // Assert
        result.Should().BeFalse();
    }

    #region Helper Methods
    private List<Cell> CreatePartialTripletCells()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0),
            Cell.CreateEmpty(1, 0),
            Cell.CreateEmpty(2, 0),
            Cell.CreateEmpty(3, 0),
            Cell.CreateEmpty(4, 0),
            Cell.CreateEmpty(5, 0),
            Cell.CreateEmpty(6, 0),
            Cell.CreateEmpty(7, 0),
            Cell.CreateEmpty(8, 0)
        };

        // Not a valid triplet pattern
        cells[0].PossibleValues.UnionWith([1, 2, 3]);
        cells[1].PossibleValues.UnionWith([1, 2]);
        cells[2].PossibleValues.UnionWith([4, 5]);
        cells[3].PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);
        cells[4].PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);
        cells[5].PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);
        cells[6].PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);
        cells[7].PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);
        cells[8].PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);

        return cells;
    }

    private List<Cell> CreateHiddenTripletCells()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0),
            Cell.CreateEmpty(1, 0),
            Cell.CreateEmpty(2, 0),
            Cell.CreateEmpty(3, 0)
        };

        // Hidden triplet [1,2,3] appears only in first 3 cells, but mixed with other values
        cells[0].PossibleValues.UnionWith([1, 4, 5, 6]);
        cells[1].PossibleValues.UnionWith([2, 4, 5, 6]);
        cells[2].PossibleValues.UnionWith([3, 4, 5, 6]);

        // Fourth cell doesn't contain any triplet numbers
        cells[3].PossibleValues.UnionWith([4, 5, 6]);

        return cells;
    }

    private List<Cell> CreateHiddenTripletWithSingleValueCell()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0),
            Cell.CreateEmpty(1, 0),
            Cell.CreateEmpty(2, 0),
            Cell.CreateEmpty(3, 0)
        };

        // Hidden triplet [1,2,3] with first cell having only 1 after reduction
        cells[0].PossibleValues.UnionWith([1, 4]); // Will become [1] after removing 4
        cells[1].PossibleValues.UnionWith([2, 5]);
        cells[2].PossibleValues.UnionWith([3, 6]);

        // Fourth cell doesn't contain triplet numbers
        cells[3].PossibleValues.UnionWith([4, 5, 6, 7]);

        return cells;
    }

    private List<Cell> CreateCellsWithoutHiddenTriplets()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0),
            Cell.CreateEmpty(1, 0),
            Cell.CreateEmpty(2, 0),
            Cell.CreateEmpty(3, 0),
            Cell.CreateEmpty(4, 0)
        };

        // No hidden triplets - spread across more than 3 cells
        cells[0].PossibleValues.UnionWith([1, 2, 4, 5]);
        cells[1].PossibleValues.UnionWith([1, 3, 6, 7]);
        cells[2].PossibleValues.UnionWith([2, 3, 8, 9]);
        cells[3].PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);
        cells[4].PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);

        return cells;
    }

    private List<Cell> CreateTripletNumbersInMoreThanThreeCells()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0),
            Cell.CreateEmpty(1, 0),
            Cell.CreateEmpty(2, 0),
            Cell.CreateEmpty(3, 0),
            Cell.CreateEmpty(4, 0),
            Cell.CreateEmpty(5, 0),
            Cell.CreateEmpty(6, 0),
            Cell.CreateEmpty(7, 0),
            Cell.CreateEmpty(8, 0)
        };

        // Triplet [1,2,3] appears in 4 cells, not exactly 3
        cells[0].PossibleValues.UnionWith([1, 4]);
        cells[1].PossibleValues.UnionWith([2, 5]);
        cells[2].PossibleValues.UnionWith([3, 6]);
        cells[3].PossibleValues.UnionWith([1, 7]);
        cells[4].PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);
        cells[5].PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);
        cells[6].PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);
        cells[7].PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);
        cells[8].PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);

        return cells;
    }

    private List<Cell> CreateTripletNumbersInLessThanThreeCells()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0),
            Cell.CreateEmpty(1, 0),
            Cell.CreateEmpty(2, 0),
            Cell.CreateEmpty(3, 0),
            Cell.CreateEmpty(4, 0),
            Cell.CreateEmpty(5, 0),
            Cell.CreateEmpty(6, 0),
            Cell.CreateEmpty(7, 0),
            Cell.CreateEmpty(8, 0)
        };

        // Triplet [1,2,3] appears in only 2 cells
        cells[0].PossibleValues.UnionWith([1, 4]);
        cells[1].PossibleValues.UnionWith([2, 5]);
        cells[2].PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);
        cells[3].PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);
        cells[4].PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);
        cells[5].PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);
        cells[6].PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);
        cells[7].PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);
        cells[8].PossibleValues.UnionWith([1, 2, 3, 4, 5, 6, 7, 8, 9]);

        return cells;
    }

    private List<Cell> CreateCellsWithMultipleHiddenTriplets()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0), // First triplet [1,2,3]
            Cell.CreateEmpty(1, 0),
            Cell.CreateEmpty(2, 0),
            Cell.CreateEmpty(3, 0), // Second triplet [4,5,6]
            Cell.CreateEmpty(4, 0),
            Cell.CreateEmpty(5, 0),
            Cell.CreateEmpty(6, 0), // Other cells
            Cell.CreateEmpty(7, 0),
            Cell.CreateEmpty(8, 0)
        };

        // First hidden triplet [1,2,3]
        cells[0].PossibleValues.UnionWith([1, 7, 8, 9]);
        cells[1].PossibleValues.UnionWith([2, 7, 8, 9]);
        cells[2].PossibleValues.UnionWith([3, 7, 8, 9]);
        
        // Second hidden triplet [4,5,6]
        cells[3].PossibleValues.UnionWith([4, 7, 8, 9]);
        cells[4].PossibleValues.UnionWith([5, 7, 8, 9]);
        cells[5].PossibleValues.UnionWith([6, 7, 8, 9]);

        // Other cells
        cells[6].PossibleValues.UnionWith([7, 8, 9]);
        cells[7].PossibleValues.UnionWith([7, 8, 9]);
        cells[8].PossibleValues.UnionWith([7, 8, 9]);

        return cells;
    }

    private List<Cell> CreateHiddenTripletWithOverlappingValues()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0),
            Cell.CreateEmpty(1, 0),
            Cell.CreateEmpty(2, 0),
            Cell.CreateEmpty(3, 0),
            Cell.CreateEmpty(4, 0),
            Cell.CreateEmpty(5, 0)
        };

        // Hidden triplet [1,2,3] with overlapping non-triplet values
        cells[0].PossibleValues.UnionWith([1, 2, 4, 5, 6]);
        cells[1].PossibleValues.UnionWith([1, 3, 4, 5, 6]);
        cells[2].PossibleValues.UnionWith([2, 3, 4, 5, 6]);

        // Fourth cell with different values
        cells[3].PossibleValues.UnionWith([7, 8, 9]);
        cells[4].PossibleValues.UnionWith([7, 8, 9]);
        cells[5].PossibleValues.UnionWith([7, 8, 9]);

        return cells;
    }

    private List<Cell> CreateCellsForAllNumberCombinations()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0),
            Cell.CreateEmpty(1, 0),
            Cell.CreateEmpty(2, 0),
            Cell.CreateEmpty(3, 0),
            Cell.CreateEmpty(4, 0),
            Cell.CreateEmpty(5, 0)
        };

        // Hidden triplet [7,8,9] to test higher number combinations
        cells[0].PossibleValues.UnionWith([7, 1, 2, 3]);
        cells[1].PossibleValues.UnionWith([8, 1, 2, 3]);
        cells[2].PossibleValues.UnionWith([9, 1, 2, 3]);

        // Other cells
        cells[3].PossibleValues.UnionWith([1, 2, 3, 4]);
        cells[4].PossibleValues.UnionWith([1, 2, 3, 5]);
        cells[5].PossibleValues.UnionWith([1, 2, 3, 6]);

        return cells;
    }

    private List<Cell> CreateHiddenTripletWithoutReduction()
    {
        var cells = new List<Cell>
        {
            Cell.CreateEmpty(0, 0),
            Cell.CreateEmpty(1, 0),
            Cell.CreateEmpty(2, 0),
            Cell.CreateEmpty(3, 0)
        };

        // Hidden triplet [1,2,3] where cells already only contain triplet values
        cells[0].PossibleValues.UnionWith([1, 2, 3]);
        cells[1].PossibleValues.UnionWith([1, 2, 3]);
        cells[2].PossibleValues.UnionWith([1, 2, 3]);

        // Fourth cell with different values
        cells[3].PossibleValues.UnionWith([4, 5, 6]);

        return cells;
    }
    #endregion
}