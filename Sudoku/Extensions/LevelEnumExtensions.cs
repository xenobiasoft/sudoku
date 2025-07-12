namespace XenobiaSoft.Sudoku.Extensions;

public static class LevelEnumExtensions
{
    public static GameDifficulty ParseLevel(this string level)
    {
        return level.ToLower() switch
        {
            "easy" => GameDifficulty.Easy,
            "medium" => GameDifficulty.Medium,
            "hard" => GameDifficulty.Hard,
            _ => throw new ArgumentException($"Invalid level: {level}")
        };
    }
}