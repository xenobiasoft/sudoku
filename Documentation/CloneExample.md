# SudokuPuzzle and Cell Cloning Implementation

This document demonstrates how to use the newly implemented cloning functionality for the `SudokuPuzzle` and `Cell` classes.

## Overview

The implementation provides deep cloning capabilities for both `SudokuPuzzle` and `Cell` objects:

- **`Cell.DeepCopy()`**: Creates a deep copy of a cell including all possible values
- **`SudokuPuzzle.Clone()`**: Creates a deep copy of a puzzle including all cells and their states

## Usage Examples

### Cloning a Cell

```csharp
using Sudoku.Domain.ValueObjects;

// Create an original cell
var originalCell = Cell.CreateEmpty(3, 4);
originalCell.AddPossibleValue(1);
originalCell.AddPossibleValue(5);
originalCell.AddPossibleValue(9);

// Clone the cell
var clonedCell = originalCell.DeepCopy();

// Modifying the clone doesn't affect the original
clonedCell.SetValue(5);

Console.WriteLine($"Original cell value: {originalCell.Value}"); // null
Console.WriteLine($"Cloned cell value: {clonedCell.Value}");     // 5
Console.WriteLine($"Original possible values: [{string.Join(", ", originalCell.PossibleValues)}]"); // 1, 5, 9
Console.WriteLine($"Cloned possible values: [{string.Join(", ", clonedCell.PossibleValues)}]");     // empty
```

### Cloning a SudokuPuzzle

```csharp
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

// Create cells for a simple puzzle
var cells = new List<Cell>();
for (int row = 0; row < 9; row++)
{
    for (int col = 0; col < 9; col++)
    {
        if (row == 0 && col == 0)
        {
            cells.Add(Cell.CreateFixed(row, col, 5));
        }
        else
        {
            cells.Add(Cell.CreateEmpty(row, col));
        }
    }
}

// Create original puzzle
var originalPuzzle = SudokuPuzzle.Create("puzzle-123", GameDifficulty.Medium, cells);

// Clone the puzzle
var clonedPuzzle = originalPuzzle.Clone();

// Modify the cloned puzzle
var cellToModify = clonedPuzzle.GetCell(1, 1);
cellToModify.SetValue(7);

// Original puzzle remains unchanged
var originalCell = originalPuzzle.GetCell(1, 1);
Console.WriteLine($"Original puzzle cell (1,1): {originalCell.Value}"); // null
Console.WriteLine($"Cloned puzzle cell (1,1): {cellToModify.Value}");   // 7
```

## Key Features

### Deep Copying

- **Cells**: All properties including `Row`, `Column`, `Value`, `IsFixed`, and `PossibleValues` are copied
- **Puzzles**: All cells are deep copied, preserving the complete state including possible values

### Independence

- Modifications to cloned objects don't affect the original
- Each `PossibleValues` collection is independently copied
- Cell state changes (like setting values) are isolated between copies

### Validation

- Cloned puzzles maintain all validation rules
- Fixed cells remain fixed in the clone
- The cloned puzzle has the same `PuzzleId` and `Difficulty` as the original

## Implementation Details

### Cell.DeepCopy()

```csharp
public Cell DeepCopy()
{
    var clonedCell = new Cell(Row, Column, Value, IsFixed);
    
    // Deep copy the possible values
    foreach (var possibleValue in PossibleValues)
    {
        clonedCell.PossibleValues.Add(possibleValue);
    }
    
    return clonedCell;
}
```

### SudokuPuzzle.Clone()

```csharp
public SudokuPuzzle Clone()
{
    // Deep clone all cells
    var clonedCells = _cells.Select(cell => cell.DeepCopy()).ToList();
    
    // Create a new puzzle instance using the private constructor
    return new SudokuPuzzle(PuzzleId, Difficulty, clonedCells);
}
```

## Use Cases

1. **Game State Backup**: Create backups before making moves that can be undone
2. **Undo/Redo Functionality**: Store previous states for undo operations
3. **AI Solving**: Create copies for testing different solving strategies
4. **Puzzle Validation**: Test validation without modifying the original puzzle
5. **Multi-Threading**: Each thread can work with its own copy of the puzzle

## Notes

- The `Cell` class uses `DeepCopy()` instead of `Clone()` because C# records don't allow methods named `Clone`
- The `SudokuPuzzle` class implements the `ICloneable<SudokuPuzzle>` interface
- All cloning operations are performed in O(n) time where n is the number of cells (81 for a standard Sudoku)