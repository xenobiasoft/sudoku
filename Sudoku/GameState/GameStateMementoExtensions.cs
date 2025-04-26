using System.Text.Json;

namespace XenobiaSoft.Sudoku.GameState;

public static class GameStateMementoExtensions
{
    public static string ToJson(this GameStateMemento memento)
    {
        return JsonSerializer.Serialize(memento);
    }

    public static GameStateMemento FromJson(this string json)
    {
        return JsonSerializer.Deserialize<GameStateMemento>(json);
    }
}