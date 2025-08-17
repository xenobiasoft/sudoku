namespace Sudoku.Web.Server.Models.Requests;

/// <summary>
/// Request model for cell operations
/// </summary>
public record CellRequest(int Row, int Column);