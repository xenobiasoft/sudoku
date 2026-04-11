namespace Sudoku.Application.DTOs;

public record ValidationResultDto(bool IsValid, List<string> Errors);