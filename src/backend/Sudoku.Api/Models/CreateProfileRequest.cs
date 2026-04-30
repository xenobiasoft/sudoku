using System.ComponentModel.DataAnnotations;

namespace Sudoku.Api.Models;

public record CreateProfileRequest([Required] string Alias);
