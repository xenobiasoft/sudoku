using Microsoft.Extensions.Logging;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Models;
using Sudoku.Application.Queries;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Enums;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Handlers;

public class GetPlayerStatsQueryHandler(
    IGameCompletionRepository completionRepository,
    IGameRepository gameRepository,
    ILogger<GetPlayerStatsQueryHandler> logger) : IQueryHandler<GetPlayerStatsQuery, PlayerStatsDto>
{
    // Fixed order so a difficulty the player has never touched still renders a row.
    private static readonly GameDifficulty[] Difficulties =
    [
        GameDifficulty.Easy,
        GameDifficulty.Medium,
        GameDifficulty.Hard,
        GameDifficulty.Expert
    ];

    // Fixed order so a (size, difficulty) combination the player has never touched still
    // renders a row; 9x9 first for readability.
    private static readonly BoardSize[] Sizes =
    [
        BoardSize.Nine,
        BoardSize.Sixteen
    ];

    public async Task<Result<PlayerStatsDto>> Handle(GetPlayerStatsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var profileId = ProfileId.From(request.ProfileId);

            var completions = (await completionRepository.GetByProfileIdAsync(profileId)).ToList();

            // Completed games are deleted by the client, but exclude any transiently-present
            // completed document so a win isn't counted by both this and its completion record.
            var activeGames = (await gameRepository.GetByProfileIdAsync(profileId))
                .Where(game => game.Status != GameStatusEnum.Completed)
                .ToList();

            var gamesWon = completions.Count;
            var gamesPlayed = gamesWon + activeGames.Count;
            var winRate = gamesPlayed == 0 ? 0 : (double)gamesWon / gamesPlayed;

            var byDifficulty = Sizes
                .SelectMany(size => Difficulties.Select(difficulty => BuildDifficultyStats(size, difficulty, completions, activeGames)))
                .ToList();

            logger.LogDebug(
                "Retrieved stats for profile {ProfileId}: {GamesPlayed} played, {GamesWon} won",
                profileId.Value, gamesPlayed, gamesWon);

            return Result<PlayerStatsDto>.Success(new PlayerStatsDto(gamesPlayed, gamesWon, winRate, byDifficulty));
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Failed to get stats for profile {ProfileId}: {Error}", request.ProfileId, ex.Message);
            return Result<PlayerStatsDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred getting stats for profile {ProfileId}", request.ProfileId);
            return Result<PlayerStatsDto>.Failure($"An unexpected error occurred: {ex.Message}");
        }
    }

    private static DifficultyStatsDto BuildDifficultyStats(
        BoardSize size,
        GameDifficulty difficulty,
        IEnumerable<GameCompletion> completions,
        IEnumerable<SudokuGame> activeGames)
    {
        var wins = completions
            .Where(completion => string.Equals(completion.Difficulty, difficulty.Name, StringComparison.OrdinalIgnoreCase)
                && completion.GridSize == size.Size)
            .ToList();
        var active = activeGames.Count(game => game.Difficulty == difficulty && game.Size == size);

        var won = wins.Count;
        var played = won + active;

        // Truncated to whole seconds: a raw tick average lands on sub-second precision
        // (averaging 1s and 2s gives 1.5s), which System.Text.Json emits as
        // "00:00:01.5000000" — breaking the "HH:MM:SS" contract the clients parse.
        TimeSpan? averageSolveTime = won == 0
            ? null
            : TruncateToSeconds(TimeSpan.FromTicks((long)wins.Average(win => win.PlayDuration.Ticks)));
        TimeSpan? bestSolveTime = won == 0
            ? null
            : TruncateToSeconds(wins.Min(win => win.PlayDuration));

        return new DifficultyStatsDto(difficulty.Name, played, won, averageSolveTime, bestSolveTime, size.Size);
    }

    private static TimeSpan TruncateToSeconds(TimeSpan value) =>
        TimeSpan.FromSeconds(Math.Floor(value.TotalSeconds));
}
