using Sudoku.Domain.Exceptions;

namespace Sudoku.Domain.ValueObjects;

public record BoardSize
{
    public static readonly BoardSize Nine = new(9, 3, 17, 3);
    public static readonly BoardSize Sixteen = new(16, 4, 55, 6);

    public int Size { get; }
    public int BoxSize { get; }
    public int MinimumClues { get; }
    public int MaxHints { get; }
    public int CellCount => Size * Size;
    public IEnumerable<int> AllValues => Enumerable.Range(1, Size);

    private BoardSize(int size, int boxSize, int minimumClues, int maxHints)
    {
        Size = size;
        BoxSize = boxSize;
        MinimumClues = minimumClues;
        MaxHints = maxHints;
    }

    public static BoardSize FromValue(int value) => value switch
    {
        9 => Nine,
        16 => Sixteen,
        _ => throw new InvalidBoardSizeException($"Invalid board size: {value}")
    };

    public override string ToString() => $"{Size}x{Size}";
}
