using System.Numerics;

namespace Sudoku.Infrastructure.Services.Solvers;

/// <summary>
/// A fast, allocation-light Sudoku engine operating on a flat <c>int[81]</c> grid
/// (0 = empty, 1-9 = value). Uses per row/column/box candidate bitmasks (bits 1-9)
/// with minimum-remaining-values (MRV) cell selection and backtracking.
///
/// This is intentionally free of domain types, DI, async, and randomness so it can be
/// reused as both a solver and a uniqueness checker, and is trivially benchmarkable.
/// </summary>
public static class BitwiseSolverEngine
{
    // Bits 1..9 set (bit 0 is unused so a value maps directly to its bit index).
    private const int FullMask = 0b11_1111_1110;

    /// <summary>
    /// Solves the grid in place, returning <c>true</c> if a solution was found.
    /// Returns the first solution discovered (MRV order). If the grid is already
    /// inconsistent (contains a duplicate), returns <c>false</c> and leaves it untouched.
    /// </summary>
    public static bool Solve(int[] grid)
    {
        var rows = new int[9];
        var cols = new int[9];
        var boxes = new int[9];

        return BuildMasks(grid, rows, cols, boxes) && SolveRecursive(grid, rows, cols, boxes);
    }

    /// <summary>
    /// Counts the number of distinct solutions, stopping as soon as <paramref name="cap"/>
    /// is reached. Uniqueness checks only need to distinguish "exactly one" from "more than
    /// one", so the default cap of 2 keeps the search cheap. The input grid is not mutated.
    /// </summary>
    public static int CountSolutions(int[] grid, int cap = 2)
    {
        var working = (int[])grid.Clone();
        var rows = new int[9];
        var cols = new int[9];
        var boxes = new int[9];

        if (!BuildMasks(working, rows, cols, boxes))
        {
            return 0;
        }

        var count = 0;
        CountRecursive(working, rows, cols, boxes, cap, ref count);

        return count;
    }

    private static bool BuildMasks(int[] grid, int[] rows, int[] cols, int[] boxes)
    {
        for (var i = 0; i < 81; i++)
        {
            var value = grid[i];
            if (value == 0)
            {
                continue;
            }

            var bit = 1 << value;
            int r = i / 9, c = i % 9, b = (r / 3) * 3 + c / 3;

            // A bit already set in any unit means a duplicate -> inconsistent grid.
            if (((rows[r] | cols[c] | boxes[b]) & bit) != 0)
            {
                return false;
            }

            rows[r] |= bit;
            cols[c] |= bit;
            boxes[b] |= bit;
        }

        return true;
    }

    private static bool SolveRecursive(int[] grid, int[] rows, int[] cols, int[] boxes)
    {
        var (index, candidates) = SelectMostConstrainedCell(grid, rows, cols, boxes);

        if (index == -1)
        {
            return true; // No empty cells remain -> solved.
        }

        if (candidates == 0)
        {
            return false; // A cell has no candidates -> dead end.
        }

        int r = index / 9, c = index % 9, b = (r / 3) * 3 + c / 3;

        while (candidates != 0)
        {
            var bit = candidates & -candidates; // lowest set bit
            candidates &= candidates - 1;
            var value = BitOperations.TrailingZeroCount(bit);

            grid[index] = value;
            rows[r] |= bit;
            cols[c] |= bit;
            boxes[b] |= bit;

            if (SolveRecursive(grid, rows, cols, boxes))
            {
                return true;
            }

            grid[index] = 0;
            rows[r] &= ~bit;
            cols[c] &= ~bit;
            boxes[b] &= ~bit;
        }

        return false;
    }

    private static void CountRecursive(int[] grid, int[] rows, int[] cols, int[] boxes, int cap, ref int count)
    {
        if (count >= cap)
        {
            return;
        }

        var (index, candidates) = SelectMostConstrainedCell(grid, rows, cols, boxes);

        if (index == -1)
        {
            count++;
            return;
        }

        if (candidates == 0)
        {
            return; // dead end
        }

        int r = index / 9, c = index % 9, b = (r / 3) * 3 + c / 3;

        while (candidates != 0 && count < cap)
        {
            var bit = candidates & -candidates;
            candidates &= candidates - 1;
            var value = BitOperations.TrailingZeroCount(bit);

            grid[index] = value;
            rows[r] |= bit;
            cols[c] |= bit;
            boxes[b] |= bit;

            CountRecursive(grid, rows, cols, boxes, cap, ref count);

            grid[index] = 0;
            rows[r] &= ~bit;
            cols[c] &= ~bit;
            boxes[b] &= ~bit;
        }
    }

    /// <summary>
    /// Finds the empty cell with the fewest candidates (MRV). Returns its index and the
    /// candidate bitmask, or (-1, 0) when the grid is full. A returned mask of 0 signals a
    /// dead end (an empty cell with no legal value).
    /// </summary>
    private static (int Index, int Candidates) SelectMostConstrainedCell(int[] grid, int[] rows, int[] cols, int[] boxes)
    {
        var bestIndex = -1;
        var bestCandidates = 0;
        var bestCount = int.MaxValue;

        for (var i = 0; i < 81; i++)
        {
            if (grid[i] != 0)
            {
                continue;
            }

            int r = i / 9, c = i % 9, b = (r / 3) * 3 + c / 3;
            var candidates = FullMask & ~(rows[r] | cols[c] | boxes[b]);
            var count = BitOperations.PopCount((uint)candidates);

            if (count < bestCount)
            {
                bestCount = count;
                bestIndex = i;
                bestCandidates = candidates;

                if (count <= 1)
                {
                    break; // Can't do better than 0 or 1 candidate.
                }
            }
        }

        return (bestIndex, bestCandidates);
    }
}
