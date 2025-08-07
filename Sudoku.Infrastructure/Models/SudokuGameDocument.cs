using System.Text.Json.Serialization;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Infrastructure.Models;

public class SudokuGameDocument
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("gameId")]
    public string GameId { get; set; } = string.Empty;
    
    [JsonPropertyName("playerAlias")]
    public string PlayerAlias { get; set; } = string.Empty;
    
    [JsonPropertyName("difficulty")]
    public GameDifficulty Difficulty { get; set; }
    
    [JsonPropertyName("status")]
    public GameStatus Status { get; set; }
    
    [JsonPropertyName("cells")]
    public List<CellDocument> Cells { get; set; } = [];
    
    [JsonPropertyName("statistics")]
    public GameStatisticsDocument Statistics { get; set; } = new();
    
    [JsonPropertyName("moveHistory")]
    public List<MoveHistoryDocument> MoveHistory { get; set; } = [];
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("startedAt")]
    public DateTime? StartedAt { get; set; }
    
    [JsonPropertyName("completedAt")]
    public DateTime? CompletedAt { get; set; }
    
    [JsonPropertyName("pausedAt")]
    public DateTime? PausedAt { get; set; }
    
    [JsonPropertyName("_partitionKey")]
    public string PartitionKey => GameId;
}

public class CellDocument
{
    [JsonPropertyName("row")]
    public int Row { get; set; }
    
    [JsonPropertyName("column")]
    public int Column { get; set; }
    
    [JsonPropertyName("value")]
    public int? Value { get; set; }
    
    [JsonPropertyName("isFixed")]
    public bool IsFixed { get; set; }
    
    [JsonPropertyName("possibleValues")]
    public HashSet<int> PossibleValues { get; set; } = [];
}

public class GameStatisticsDocument
{
    [JsonPropertyName("totalMoves")]
    public int TotalMoves { get; set; }
    
    [JsonPropertyName("validMoves")]
    public int ValidMoves { get; set; }
    
    [JsonPropertyName("invalidMoves")]
    public int InvalidMoves { get; set; }
    
    [JsonPropertyName("playDuration")]
    public TimeSpan PlayDuration { get; set; }
    
    [JsonPropertyName("lastMoveAt")]
    public DateTime? LastMoveAt { get; set; }
}

public class MoveHistoryDocument
{
    [JsonPropertyName("row")]
    public int Row { get; set; }
    
    [JsonPropertyName("column")]
    public int Column { get; set; }
    
    [JsonPropertyName("previousValue")]
    public int? PreviousValue { get; set; }
    
    [JsonPropertyName("newValue")]
    public int? NewValue { get; set; }
}