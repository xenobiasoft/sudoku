namespace Sudoku.Domain.ValueObjects;

public record GameDifficulty
{
    public static readonly GameDifficulty Easy = new(1, "Easy");
    public static readonly GameDifficulty Medium = new(2, "Medium");
    public static readonly GameDifficulty Hard = new(3, "Hard");
    public static readonly GameDifficulty Expert = new(4, "Expert");

    public int Value { get; }
    public string Name { get; }

    private GameDifficulty(int value, string name)
    {
        Value = value;
        Name = name;
    }

    public static GameDifficulty FromValue(int value) => value switch
    {
        1 => Easy,
        2 => Medium,
        3 => Hard,
        4 => Expert,
        _ => throw new InvalidGameDifficultyException($"Invalid difficulty value: {value}")
    };

    public static GameDifficulty FromName(string name) => name?.ToLowerInvariant() switch
    {
        "easy" => Easy,
        "medium" => Medium,
        "hard" => Hard,
        "expert" => Expert,
        _ => throw new InvalidGameDifficultyException($"Invalid difficulty name: {name}")
    };

    public static implicit operator int(GameDifficulty difficulty) => difficulty.Value;
    public static implicit operator string(GameDifficulty difficulty) => difficulty.Name;

    public override string ToString() => Name;
}