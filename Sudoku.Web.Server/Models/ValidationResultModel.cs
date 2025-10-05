namespace Sudoku.Web.Server.Models;

/// <summary>
/// Represents the result of game validation
/// </summary>
public record ValidationResultModel(
    bool IsValid,
    bool IsComplete,
    List<string> Errors);