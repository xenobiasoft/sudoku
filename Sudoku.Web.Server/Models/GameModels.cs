namespace Sudoku.Web.Server.Models;

/// <summary>
/// Represents a game in the Blazor application
/// </summary>
public record GameModel(
    string Id,
    string PlayerAlias,
    string Difficulty,
    string Status,
    GameStatisticsModel Statistics,
    DateTime CreatedAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    DateTime? PausedAt,
    List<CellModel> Cells);

/// <summary>
/// Represents game statistics
/// </summary>
public record GameStatisticsModel(
    int TotalMoves,
    int ValidMoves,
    int InvalidMoves,
    TimeSpan PlayDuration,
    double AccuracyPercentage);

/// <summary>
/// Represents a cell in the game
/// </summary>
public record CellModel(
    int Row,
    int Column,
    int? Value,
    bool IsFixed,
    bool HasValue,
    List<int> PossibleValues);

/// <summary>
/// Represents the result of game validation
/// </summary>
public record ValidationResultModel(
    bool IsValid,
    bool IsComplete,
    List<string> Errors);

/// <summary>
/// Request model for creating a player
/// </summary>
public record CreatePlayerRequest(string? Alias);

/// <summary>
/// Request model for making a move
/// </summary>
public record MoveRequest(int Row, int Column, int? Value);

/// <summary>
/// Request model for possible value operations
/// </summary>
public record PossibleValueRequest(int Row, int Column, int Value);

/// <summary>
/// Request model for cell operations
/// </summary>
public record CellRequest(int Row, int Column);