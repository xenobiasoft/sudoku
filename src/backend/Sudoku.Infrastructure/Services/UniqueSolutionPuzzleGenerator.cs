using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Services.Solvers;
using Sudoku.Infrastructure.Utilities;

namespace Sudoku.Infrastructure.Services;

/// <summary>
/// An <see cref="IPuzzleGenerator"/> that guarantees every generated puzzle has exactly
/// one solution. It builds a complete grid with <see cref="BitwiseSolverEngine"/>, then
/// digs holes, keeping a removal only while the puzzle still has a unique solution
/// (verified via <see cref="BitwiseSolverEngine.CountSolutions"/>).
///
/// Digging runs in two passes: symmetric mirrored pairs first (an aesthetic), then single
/// cells to reach the target. The second pass is what lets the deeper difficulties actually
/// hit their band — a symmetric-only dig stalls around 50 empty cells, which would leave
/// Expert indistinguishable from Hard.
///
/// Empty-cell targets per difficulty mirror the legacy <see cref="PuzzleGenerator"/> for 9x9
/// so the two are directly comparable. The 16x16 Easy/Medium bands are the proportional
/// scale-ups from FR-5 in the 16x16 spec, verified fast (well under a second for Easy, low
/// tens of seconds worst-case for Medium). The 16x16 Hard/Expert bands were tuned DOWN from
/// the spec's proportional estimate (Hard 158-168, Expert 171-183) after Phase 2 measurement
/// showed digging past ~160 empty cells costs minutes per removal and can stall before
/// reaching the target at all within a single dig pass (uniqueness re-verification on a
/// near-minimal 256-cell grid is combinatorially expensive, and the achievable ceiling for a
/// single greedy pass is seed-dependent) — see the Phase 2 implementation notes for measured
/// timings. The tuned bands below keep worst-case generation within the user-approved ~2
/// minute budget (per the spec's §11 policy: tune the band down rather than optimize the
/// generator further) while preserving a monotonically increasing difficulty progression.
/// </summary>
public class UniqueSolutionPuzzleGenerator : IPuzzleGenerator
{
    private const int NINE_EASY_EMPTY_MIN = 40;
    private const int NINE_EASY_EMPTY_MAX = 45;
    private const int NINE_MEDIUM_EMPTY_MIN = 46;
    private const int NINE_MEDIUM_EMPTY_MAX = 49;
    private const int NINE_HARD_EMPTY_MIN = 50;
    private const int NINE_HARD_EMPTY_MAX = 53;
    private const int NINE_EXPERT_EMPTY_MIN = 54;
    private const int NINE_EXPERT_EMPTY_MAX = 58;

    private const int SIXTEEN_EASY_EMPTY_MIN = 126;
    private const int SIXTEEN_EASY_EMPTY_MAX = 142;
    private const int SIXTEEN_MEDIUM_EMPTY_MIN = 145;
    private const int SIXTEEN_MEDIUM_EMPTY_MAX = 155;

    // Tuned down from the spec's proportional estimate of 158-168 (see the Phase 2
    // measurement notes above): 158 measured ~45s, but the band's upper end was seed-
    // dependent and could take minutes or fail to be reached in a single dig pass.
    private const int SIXTEEN_HARD_EMPTY_MIN = 156;
    private const int SIXTEEN_HARD_EMPTY_MAX = 162;

    // Tuned down from the spec's proportional estimate of 171-183 (see the Phase 2
    // measurement notes above): that range was not reliably reachable within the ~2 minute
    // budget with the current single-pass dig algorithm. This band sits just past Hard's,
    // deliberately conservative given the steep, seed-dependent cost curve observed near
    // the practical dig ceiling for 16x16.
    private const int SIXTEEN_EXPERT_EMPTY_MIN = 163;
    private const int SIXTEEN_EXPERT_EMPTY_MAX = 170;

    public Task<SudokuPuzzle> GeneratePuzzleAsync(GameDifficulty difficulty, BoardSize size)
    {
        var grid = GenerateFullSolution(size);
        var targetEmpty = GetTargetEmptyCount(difficulty, size);

        DigHoles(grid, targetEmpty, size);

        return Task.FromResult(BuildPuzzle(grid, difficulty, size));
    }

    private static int[] GenerateFullSolution(BoardSize size)
    {
        var grid = new int[size.CellCount];

        // Seeding the diagonal boxes is always conflict-free (they share no row, column, or
        // box), and the random fill is what gives each generated puzzle a different solution
        // once the solver completes the rest.
        for (var box = 0; box < size.BoxSize; box++)
        {
            var values = ShuffledOneToSize(size);
            var k = 0;

            for (var r = box * size.BoxSize; r < box * size.BoxSize + size.BoxSize; r++)
            {
                for (var c = box * size.BoxSize; c < box * size.BoxSize + size.BoxSize; c++)
                {
                    grid[r * size.Size + c] = values[k++];
                }
            }
        }

        BitwiseSolverEngine.Solve(grid);

        return grid;
    }

    private static void DigHoles(int[] grid, int targetEmpty, BoardSize size)
    {
        var removed = DigPass(grid, targetEmpty, removed: 0, symmetric: true, size);

        // A symmetric pass alone plateaus well short of the deeper targets: near the
        // uniqueness limit, clearing a mirrored pair costs uniqueness roughly twice as often
        // as clearing one cell, so every surviving pair gets rejected while single cells would
        // still come out. That leaves the deeper targets permanently short. Symmetry is only
        // an aesthetic; the clue count is what makes a puzzle hard — so finish one cell at a
        // time. Shallower difficulties reach their target in the pass above and skip this.
        if (removed < targetEmpty)
        {
            DigPass(grid, targetEmpty, removed, symmetric: false, size);
        }
    }

    private static int DigPass(int[] grid, int targetEmpty, int removed, bool symmetric, BoardSize size)
    {
        foreach (var index in ShuffledPositions(size))
        {
            var remaining = targetEmpty - removed;
            if (remaining <= 0)
            {
                break;
            }

            if (grid[index] == 0)
            {
                continue; // already cleared (e.g. as a mirror)
            }

            var mirror = size.CellCount - 1 - index;

            // Remove a symmetric pair when there's room for two; otherwise a single cell.
            // Bounding the group by 'remaining' guarantees we never exceed the target.
            var group = (!symmetric || index == mirror || grid[mirror] == 0 || remaining < 2)
                ? new[] { index }
                : new[] { index, mirror };

            var saved = group.Select(i => grid[i]).ToArray();

            foreach (var i in group)
            {
                grid[i] = 0;
            }

            if (BitwiseSolverEngine.CountSolutions(grid, cap: 2) == 1)
            {
                removed += group.Length;
            }
            else
            {
                for (var i = 0; i < group.Length; i++)
                {
                    grid[group[i]] = saved[i];
                }
            }
        }

        return removed;
    }

    private static SudokuPuzzle BuildPuzzle(int[] grid, GameDifficulty difficulty, BoardSize size)
    {
        var cells = Enumerable.Range(0, size.CellCount)
            .Select(i =>
            {
                int row = i / size.Size, column = i % size.Size;
                return grid[i] == 0
                    ? Cell.CreateEmpty(row, column, size)
                    : Cell.CreateFixed(row, column, grid[i], size);
            })
            .ToList();

        return SudokuPuzzle.Create(GameId.New(), difficulty, size, cells);
    }

    private static int GetTargetEmptyCount(GameDifficulty difficulty, BoardSize size)
    {
        if (size == BoardSize.Sixteen)
        {
            return difficulty.Name switch
            {
                "Easy" => RandomGenerator.RandomNumber(SIXTEEN_EASY_EMPTY_MIN, SIXTEEN_EASY_EMPTY_MAX),
                "Medium" => RandomGenerator.RandomNumber(SIXTEEN_MEDIUM_EMPTY_MIN, SIXTEEN_MEDIUM_EMPTY_MAX),
                "Hard" => RandomGenerator.RandomNumber(SIXTEEN_HARD_EMPTY_MIN, SIXTEEN_HARD_EMPTY_MAX),
                "Expert" => RandomGenerator.RandomNumber(SIXTEEN_EXPERT_EMPTY_MIN, SIXTEEN_EXPERT_EMPTY_MAX),
                _ => 0
            };
        }

        return difficulty.Name switch
        {
            "Easy" => RandomGenerator.RandomNumber(NINE_EASY_EMPTY_MIN, NINE_EASY_EMPTY_MAX),
            "Medium" => RandomGenerator.RandomNumber(NINE_MEDIUM_EMPTY_MIN, NINE_MEDIUM_EMPTY_MAX),
            "Hard" => RandomGenerator.RandomNumber(NINE_HARD_EMPTY_MIN, NINE_HARD_EMPTY_MAX),
            "Expert" => RandomGenerator.RandomNumber(NINE_EXPERT_EMPTY_MIN, NINE_EXPERT_EMPTY_MAX),
            _ => 0
        };
    }

    private static int[] ShuffledOneToSize(BoardSize size)
    {
        var values = Enumerable.Range(1, size.Size).ToArray();
        Shuffle(values);
        return values;
    }

    private static int[] ShuffledPositions(BoardSize size)
    {
        var positions = Enumerable.Range(0, size.CellCount).ToArray();
        Shuffle(positions);
        return positions;
    }

    private static void Shuffle(int[] array)
    {
        for (var i = array.Length - 1; i > 0; i--)
        {
            var j = RandomGenerator.RandomNumber(0, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}
