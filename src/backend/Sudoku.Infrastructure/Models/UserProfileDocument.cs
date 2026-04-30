using Newtonsoft.Json;

namespace Sudoku.Infrastructure.Models;

public class UserProfileDocument
{
    [JsonProperty("id")]        public string Id { get; set; } = string.Empty;
    [JsonProperty("profileId")] public string ProfileId { get; set; } = string.Empty;
    [JsonProperty("alias")]     public string Alias { get; set; } = string.Empty;
    [JsonProperty("createdAt")] public DateTime CreatedAt { get; set; }
    [JsonProperty("updatedAt")] public DateTime UpdatedAt { get; set; }
    [JsonProperty("lockedAt")]  public DateTime? LockedAt { get; set; }
    [JsonProperty("lockToken")] public string? LockToken { get; set; }
}
