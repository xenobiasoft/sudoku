using Newtonsoft.Json;

namespace Sudoku.Infrastructure.Models;

public class GameCompletionDocument
{
    [JsonProperty("id")]           public string Id { get; set; } = string.Empty;
    [JsonProperty("gameId")]       public string GameId { get; set; } = string.Empty;
    [JsonProperty("profileId")]    public string ProfileId { get; set; } = string.Empty;
    [JsonProperty("difficulty")]   public string Difficulty { get; set; } = string.Empty;
    [JsonProperty("gridSize")]     public int GridSize { get; set; } = 9;
    [JsonProperty("playDuration")] public TimeSpan PlayDuration { get; set; }
    [JsonProperty("completedAt")]  public DateTime CompletedAt { get; set; }
}
