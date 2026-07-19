namespace Sudoku.Application.Models;

/// <summary>
/// A compact, durable record of a won game. Written when a game completes and keyed by
/// <see cref="GameId"/> so writes are idempotent. Survives deletion of the full game
/// document, which the client issues moments after a puzzle is solved.
/// </summary>
public record GameCompletion(
    string GameId,
    string ProfileId,
    string Difficulty,
    TimeSpan PlayDuration,
    DateTime CompletedAt,
    int GridSize);
