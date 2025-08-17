namespace Sudoku.Web.Server.Models;

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