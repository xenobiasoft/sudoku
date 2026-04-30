using System.ComponentModel.DataAnnotations;

namespace Sudoku.Api.Models;

public record UpdateProfileAliasRequest([Required] string NewAlias);
