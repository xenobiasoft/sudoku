using Sudoku.Domain.Exceptions;

namespace Sudoku.Domain.ValueObjects;

public record BoardSize
{
    // Declared before the board instances so the static initializers below see populated arrays.
    private static readonly GameDifficulty[] AllDifficulties =
        [GameDifficulty.Easy, GameDifficulty.Medium, GameDifficulty.Hard, GameDifficulty.Expert];

    // Hard/Expert digging on a 256-cell grid runs for minutes per puzzle and is highly
    // seed-dependent, so the pool can never be kept stocked. Those two are not offered at 16x16.
    private static readonly GameDifficulty[] SixteenDifficulties =
        [GameDifficulty.Easy, GameDifficulty.Medium];

    public static readonly BoardSize Nine = new(9, 3, 17, 3, AllDifficulties);
    public static readonly BoardSize Sixteen = new(16, 4, 55, 6, SixteenDifficulties);

    public int Size { get; }
    public int BoxSize { get; }
    public int MinimumClues { get; }
    public int MaxHints { get; }
    public int CellCount => Size * Size;
    public IEnumerable<int> AllValues => Enumerable.Range(1, Size);
    public IReadOnlyList<GameDifficulty> SupportedDifficulties { get; }

    private BoardSize(int size, int boxSize, int minimumClues, int maxHints, IReadOnlyList<GameDifficulty> supportedDifficulties)
    {
        Size = size;
        BoxSize = boxSize;
        MinimumClues = minimumClues;
        MaxHints = maxHints;
        SupportedDifficulties = supportedDifficulties;
    }

    public bool Supports(GameDifficulty difficulty) => SupportedDifficulties.Contains(difficulty);

    public static BoardSize FromValue(int value) => value switch
    {
        9 => Nine,
        16 => Sixteen,
        _ => throw new InvalidBoardSizeException($"Invalid board size: {value}")
    };

    public override string ToString() => $"{Size}x{Size}";
}
