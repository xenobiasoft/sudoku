using Newtonsoft.Json;
using Sudoku.Domain.Enums;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Infrastructure.Models;

public class SudokuGameDocument
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("gameId")]
    public string GameId { get; set; } = string.Empty;

    [JsonProperty("playerAlias")]
    public string PlayerAlias { get; set; } = string.Empty;

    [JsonProperty("difficulty")]
    public GameDifficulty Difficulty { get; set; } = GameDifficulty.Easy;

    [JsonProperty("status")]
    public GameStatus Status { get; set; }
    
    [JsonProperty("cells")]
    public List<CellDocument> Cells { get; set; } = [];
    
    [JsonProperty("statistics")]
    public GameStatisticsDocument Statistics { get; set; } = new();
    
    [JsonProperty("moveHistory")]
    public List<MoveHistoryDocument> MoveHistory { get; set; } = [];
    
    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [JsonProperty("startedAt")]
    public DateTime? StartedAt { get; set; }
    
    [JsonProperty("completedAt")]
    public DateTime? CompletedAt { get; set; }
    
    [JsonProperty("pausedAt")]
    public DateTime? PausedAt { get; set; }
}

public class CellDocument
{
    [JsonProperty("row")]
    public int Row { get; set; }
    
    [JsonProperty("column")]
    public int Column { get; set; }
    
    [JsonProperty("value")]
    public int? Value { get; set; }
    
    [JsonProperty("isFixed")]
    public bool IsFixed { get; set; }
    
    [JsonProperty("possibleValues")]
    public HashSet<int> PossibleValues { get; set; } = [];
}

public class GameStatisticsDocument
{
    [JsonProperty("totalMoves")]
    public int TotalMoves { get; set; }
    
    [JsonProperty("validMoves")]
    public int ValidMoves { get; set; }
    
    [JsonProperty("invalidMoves")]
    public int InvalidMoves { get; set; }
    
    [JsonProperty("playDuration")]
    public TimeSpan PlayDuration { get; set; }
    
    [JsonProperty("lastMoveAt")]
    public DateTime? LastMoveAt { get; set; }
}

public class MoveHistoryDocument
{
    [JsonProperty("row")]
    public int Row { get; set; }
    
    [JsonProperty("column")]
    public int Column { get; set; }
    
    [JsonProperty("previousValue")]
    public int? PreviousValue { get; set; }
    
    [JsonProperty("newValue")]
    public int? NewValue { get; set; }
}