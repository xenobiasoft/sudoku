namespace Sudoku.Web.Server.Helpers;

public static class LevelEnumExtensions
{
    public static Level ParseLevel(this string level)
    {
        return level.ToLower() switch
        {
            "easy" => Level.Easy,
            "medium" => Level.Medium,
            "hard" => Level.Hard,
            _ => throw new ArgumentException($"Invalid level: {level}")
        };
    }
}