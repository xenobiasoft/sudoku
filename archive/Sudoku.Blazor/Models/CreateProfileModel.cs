using System.ComponentModel.DataAnnotations;

namespace Sudoku.Blazor.Models;

public class CreateProfileModel
{
    [Required(ErrorMessage = "Alias is required.")]
    [MinLength(2, ErrorMessage = "Alias must be at least 2 characters.")]
    [MaxLength(50, ErrorMessage = "Alias cannot exceed 50 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Alias can only contain letters, numbers, and spaces.")]
    public string Alias { get; set; } = string.Empty;
}
