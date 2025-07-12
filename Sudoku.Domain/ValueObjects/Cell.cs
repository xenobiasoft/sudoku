using Sudoku.Domain.Exceptions;

namespace Sudoku.Domain.ValueObjects;

public record Cell
{
    public int Row { get; }
    public int Column { get; }
    public int? Value { get; private set; }
    public bool IsFixed { get; }
    public bool HasValue => Value.HasValue;

    private Cell(int row, int column, int? value, bool isFixed)
    {
        if (row < 0 || row > 8)
            throw new InvalidCellPositionException($"Row must be between 0 and 8, got: {row}");

        if (column < 0 || column > 8)
            throw new InvalidCellPositionException($"Column must be between 0 and 8, got: {column}");

        if (value.HasValue && (value.Value < 1 || value.Value > 9))
            throw new InvalidCellValueException($"Cell value must be between 1 and 9, got: {value.Value}");

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
            throw new CellIsFixedException($"Cannot modify fixed cell at position ({Row}, {Column})");

        if (value < 1 || value > 9)
            throw new InvalidCellValueException($"Cell value must be between 1 and 9, got: {value}");

        Value = value;
    }

    public void ClearValue()
    {
        if (IsFixed)
            throw new CellIsFixedException($"Cannot modify fixed cell at position ({Row}, {Column})");

        Value = null;
    }

    public override string ToString()
    {
        return Value?.ToString() ?? ".";
    }
}