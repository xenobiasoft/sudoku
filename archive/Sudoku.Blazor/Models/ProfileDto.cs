namespace Sudoku.Blazor.Models;

public class ProfileDto
{
    public string ProfileId { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
