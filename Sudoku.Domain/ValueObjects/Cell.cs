using Sudoku.Domain.Exceptions;

namespace Sudoku.Domain.ValueObjects;

public record Cell
{
    public int Row { get; }
    public int Column { get; }
    public int? Value { get; private set; }
    public bool IsFixed { get; }
    public bool HasValue => Value.HasValue;
    public HashSet<int> PossibleValues { get; private set; } = new();

    private Cell(int row, int column, int? value, bool isFixed)
    {
        if (row < 0 || row > 8)
        {
            throw new InvalidCellPositionException($"Row must be between 0 and 8, got: {row}");
        }

        if (column < 0 || column > 8)
        {
            throw new InvalidCellPositionException($"Column must be between 0 and 8, got: {column}");
        }

        if (value.HasValue && (value.Value < 1 || value.Value > 9))
        {
            throw new InvalidCellValueException($"Cell value must be between 1 and 9, got: {value.Value}");
        }

        Row = row;
        Column = column;
        Value = value;
        IsFixed = isFixed;
    }

    public static Cell Create(int row, int column, int? value = null, bool isFixed = false)
    {
        return new Cell(row, column, value, isFixed);
    }

    public static Cell CreateFixed(int row, int column, int value)
    {
        return new Cell(row, column, value, true);
    }

    public static Cell CreateEmpty(int row, int column)
    {
        return new Cell(row, column, null, false);
    }

    public void SetValue(int value)
    {
        if (IsFixed)
        {
            throw new CellIsFixedException($"Cannot modify fixed cell at position ({Row}, {Column})");
        }

        if (value < 1 || value > 9)
        {
            throw new InvalidCellValueException($"Cell value must be between 1 and 9, got: {value}");
        }

        Value = value;
        // Clear possible values when a definite value is set
        PossibleValues.Clear();
    }

    public void SetValue(int? value)
    {
        if (IsFixed)
        {
            throw new CellIsFixedException($"Cannot modify fixed cell at position ({Row}, {Column})");
        }

        if (value.HasValue)
        {
            if (value.Value < 1 || value.Value > 9)
            {
                throw new InvalidCellValueException($"Cell value must be between 1 and 9, got: {value.Value}");
            }
        }

        Value = value;

        // Clear possible values when a definite value is set
        if (value.HasValue)
        {
            PossibleValues.Clear();
        }
    }

    public void ClearValue()
    {
        if (IsFixed)
        {
            throw new CellIsFixedException($"Cannot modify fixed cell at position ({Row}, {Column})");
        }

        Value = null;
    }

    public void AddPossibleValue(int value)
    {
        if (IsFixed)
        {
            throw new CellIsFixedException($"Cannot modify fixed cell at position ({Row}, {Column})");
        }

        if (Value.HasValue)
        {
            throw new InvalidOperationException($"Cannot add possible values to cell with a definite value at position ({Row}, {Column})");
        }

        if (value < 1 || value > 9)
        {
            throw new InvalidCellValueException($"Possible value must be between 1 and 9, got: {value}");
        }

        if (!PossibleValues.Contains(value))
        {
            PossibleValues.Add(value);
        }
    }

    public void RemovePossibleValue(int value)
    {
        if (IsFixed)
        {
            throw new CellIsFixedException($"Cannot modify fixed cell at position ({Row}, {Column})");
        }

        if (value < 1 || value > 9)
        {
            throw new InvalidCellValueException($"Possible value must be between 1 and 9, got: {value}");
        }

        PossibleValues.Remove(value);
    }

    public void ClearPossibleValues()
    {
        if (IsFixed)
        {
            throw new CellIsFixedException($"Cannot modify fixed cell at position ({Row}, {Column})");
        }

        PossibleValues.Clear();
    }

    public override string ToString()
    {
        return Value?.ToString() ?? ".";
    }

    /// <summary>
    /// Creates a deep copy of the current cell.
    /// </summary>
    /// <returns>A deep copy of the current cell with all possible values copied.</returns>
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
}