namespace Sudoku.Application.DTOs;

public record PlayerStatsDto(
    int GamesPlayed,
    int GamesWon,
    double WinRate,
    IReadOnlyList<DifficultyStatsDto> ByDifficulty);

public record DifficultyStatsDto(
    string Difficulty,
    int GamesPlayed,
    int GamesWon,
    TimeSpan? AverageSolveTime,
    TimeSpan? BestSolveTime);
