using System.ComponentModel.DataAnnotations;

namespace Sudoku.Blazor.Models;

public class EditProfileModel
{
    [Required(ErrorMessage = "Profile DisplayName is required.")]
    [MinLength(2, ErrorMessage = "Profile DisplayName must be at least 2 characters.")]
    [MaxLength(50, ErrorMessage = "Profile DisplayName cannot exceed 50 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Profile DisplayName can only contain letters, numbers, and spaces.")]
    public string DisplayName { get; set; } = string.Empty;
}