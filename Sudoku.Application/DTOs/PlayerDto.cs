namespace Sudoku.Application.DTOs;

public record PlayerDto(
    string Alias,
    int TotalGames,
    int CompletedGames,
    int InProgressGames,
    int PausedGames,
    int AbandonedGames,
    DateTime LastPlayedAt);