using Sudoku.Domain.ValueObjects;

namespace Sudoku.Functions.Services;

/// <summary>
/// Narrows a seeding run to a subset of the board-size/difficulty matrix. Every member is
/// optional; a null member means "no restriction on this dimension". Used by the on-demand
/// HTTP seed so a developer can top up a single pool (e.g. 16x16 Easy) without waiting for
/// the whole matrix.
/// </summary>
/// <param name="Size">Restrict to a single board size, or null for every size.</param>
/// <param name="Difficulty">Restrict to a single difficulty, or null for every difficulty.</param>
/// <param name="Target">Override the per-combination target pool size, or null for the default.</param>
public record PuzzlePoolSeedFilter(BoardSize? Size = null, GameDifficulty? Difficulty = null, int? Target = null);
