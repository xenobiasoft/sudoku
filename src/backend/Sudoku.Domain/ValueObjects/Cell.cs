using Sudoku.Domain.Exceptions;

namespace Sudoku.Domain.ValueObjects;

public record Cell
{
    public int Row { get; }
    public int Column { get; }
    public int? Value { get; private set; }
    public bool IsFixed { get; }
    public bool IsHint { get; }
    public BoardSize Size { get; }
    public bool HasValue => Value.HasValue;

    /// <summary>
    /// A cell is locked when it cannot be modified by the player: original clues (<see cref="IsFixed"/>)
    /// and cells revealed by a hint (<see cref="IsHint"/>).
    /// </summary>
    public bool IsLocked => IsFixed || IsHint;
    public HashSet<int> PossibleValues { get; private set; } = new();

    private Cell(int row, int column, int? value, bool isFixed, BoardSize size, bool isHint = false)
    {
        if (row < 0 || row > size.Size - 1)
        {
            throw new InvalidCellPositionException($"Row must be between 0 and {size.Size - 1}, got: {row}");
        }

        if (column < 0 || column > size.Size - 1)
        {
            throw new InvalidCellPositionException($"Column must be between 0 and {size.Size - 1}, got: {column}");
        }

        if (value.HasValue && (value.Value < 1 || value.Value > size.Size))
        {
            throw new InvalidCellValueException($"Cell value must be between 1 and {size.Size}, got: {value.Value}");
        }

        Row = row;
        Column = column;
        Value = value;
        IsFixed = isFixed;
        IsHint = isHint;
        Size = size;
    }

    public static Cell Create(int row, int column, BoardSize size, int? value = null, bool isFixed = false, bool isHint = false)
    {
        return new Cell(row, column, value, isFixed, size, isHint);
    }

    public static Cell CreateFixed(int row, int column, int value, BoardSize size)
    {
        return new Cell(row, column, value, true, size);
    }

    public static Cell CreateEmpty(int row, int column, BoardSize size)
    {
        return new Cell(row, column, null, false, size);
    }

    /// <summary>
    /// Creates a cell revealed by a hint: it holds the correct value and is locked so the player
    /// cannot change it, but it is distinct from an original clue (<see cref="IsFixed"/> stays false).
    /// </summary>
    public static Cell CreateHint(int row, int column, int value, BoardSize size)
    {
        return new Cell(row, column, value, isFixed: false, size, isHint: true);
    }

    public void SetValue(int value)
    {
        if (IsLocked)
        {
            throw new CellIsFixedException($"Cannot modify fixed cell at position ({Row}, {Column})");
        }

        if (value < 1 || value > Size.Size)
        {
            throw new InvalidCellValueException($"Cell value must be between 1 and {Size.Size}, got: {value}");
        }

        Value = value;
        PossibleValues.Clear();
    }

    public void SetValue(int? value)
    {
        if (IsLocked)
        {
            throw new CellIsFixedException($"Cannot modify fixed cell at position ({Row}, {Column})");
        }

        if (value.HasValue)
        {
            if (value.Value < 1 || value.Value > Size.Size)
            {
                throw new InvalidCellValueException($"Cell value must be between 1 and {Size.Size}, got: {value.Value}");
            }
        }

        Value = value;

        if (value.HasValue)
        {
            PossibleValues.Clear();
        }
    }

    public void ClearValue()
    {
        if (IsLocked)
        {
            throw new CellIsFixedException($"Cannot modify fixed cell at position ({Row}, {Column})");
        }

        Value = null;
    }

    public void AddPossibleValue(int value)
    {
        if (IsLocked)
        {
            throw new CellIsFixedException($"Cannot modify fixed cell at position ({Row}, {Column})");
        }

        if (Value.HasValue)
        {
            throw new CellAlreadyHasValueException($"Cannot add possible values to cell with a definite value at position ({Row}, {Column})");
        }

        if (value < 1 || value > Size.Size)
        {
            throw new InvalidCellValueException($"Possible value must be between 1 and {Size.Size}, got: {value}");
        }

        if (!PossibleValues.Contains(value))
        {
            PossibleValues.Add(value);
        }
    }

    public void RemovePossibleValue(int value)
    {
        if (IsLocked)
        {
            throw new CellIsFixedException($"Cannot modify fixed cell at position ({Row}, {Column})");
        }

        if (value < 1 || value > Size.Size)
        {
            throw new InvalidCellValueException($"Possible value must be between 1 and {Size.Size}, got: {value}");
        }

        PossibleValues.Remove(value);
    }

    public void ClearPossibleValues()
    {
        if (IsLocked)
        {
            throw new CellIsFixedException($"Cannot modify fixed cell at position ({Row}, {Column})");
        }

        PossibleValues.Clear();
    }

    public override string ToString()
    {
        return Value?.ToString() ?? ".";
    }

    public Cell DeepCopy()
    {
        var clonedCell = new Cell(Row, Column, Value, IsFixed, Size, IsHint);

        foreach (var possibleValue in PossibleValues)
        {
            clonedCell.PossibleValues.Add(possibleValue);
        }

        return clonedCell;
    }
}
