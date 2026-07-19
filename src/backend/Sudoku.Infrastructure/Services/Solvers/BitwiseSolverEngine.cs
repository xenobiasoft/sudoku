using System.Numerics;

namespace Sudoku.Infrastructure.Services.Solvers;

/// <summary>
/// A fast, allocation-light Sudoku engine operating on a flat <c>int[]</c> grid
/// (0 = empty, 1-N = value), where the board size is inferred from the grid length:
/// <c>81</c> (9x9, 3x3 boxes) or <c>256</c> (16x16, 4x4 boxes). Uses per row/column/box
/// candidate bitmasks (bits 1-size) with minimum-remaining-values (MRV) cell selection
/// and backtracking.
///
/// This is intentionally free of domain types, DI, async, and randomness so it can be
/// reused as both a solver and a uniqueness checker, and is trivially benchmarkable.
/// </summary>
public static class BitwiseSolverEngine
{
    /// <summary>
    /// Solves the grid in place, returning <c>true</c> if a solution was found.
    /// Returns the first solution discovered (MRV order). If the grid is already
    /// inconsistent (contains a duplicate), returns <c>false</c> and leaves it untouched.
    /// </summary>
    public static bool Solve(int[] grid)
    {
        var (size, boxSize) = ResolveDimensions(grid);
        var rows = new int[size];
        var cols = new int[size];
        var boxes = new int[size];
        var fullMask = BuildFullMask(size);

        return BuildMasks(grid, rows, cols, boxes, size, boxSize)
            && SolveRecursive(grid, rows, cols, boxes, size, boxSize, fullMask);
    }

    /// <summary>
    /// Counts the number of distinct solutions, stopping as soon as <paramref name="cap"/>
    /// is reached. Uniqueness checks only need to distinguish "exactly one" from "more than
    /// one", so the default cap of 2 keeps the search cheap. The input grid is not mutated.
    /// </summary>
    public static int CountSolutions(int[] grid, int cap = 2)
    {
        var (size, boxSize) = ResolveDimensions(grid);
        var working = (int[])grid.Clone();
        var rows = new int[size];
        var cols = new int[size];
        var boxes = new int[size];
        var fullMask = BuildFullMask(size);

        if (!BuildMasks(working, rows, cols, boxes, size, boxSize))
        {
            return 0;
        }

        var count = 0;
        CountRecursive(working, rows, cols, boxes, size, boxSize, fullMask, cap, ref count);

        return count;
    }

    /// <summary>
    /// Infers the board size and box size from the grid length: 81 -> 9 (3x3 boxes),
    /// 256 -> 16 (4x4 boxes). Any other length is not a supported board shape.
    /// </summary>
    private static (int Size, int BoxSize) ResolveDimensions(int[] grid) => grid.Length switch
    {
        81 => (9, 3),
        256 => (16, 4),
        _ => throw new ArgumentException(
            $"Unsupported grid length: {grid.Length}. Expected 81 (9x9) or 256 (16x16).", nameof(grid))
    };

    /// <summary>Bits 1..size set (bit 0 is unused so a value maps directly to its bit index).</summary>
    private static int BuildFullMask(int size) => (1 << (size + 1)) - 2;

    private static bool BuildMasks(int[] grid, int[] rows, int[] cols, int[] boxes, int size, int boxSize)
    {
        for (var i = 0; i < grid.Length; i++)
        {
            var value = grid[i];
            if (value == 0)
            {
                continue;
            }

            if (value < 1 || value > size)
            {
                return false; // Out-of-range value -> inconsistent grid.
            }

            var bit = 1 << value;
            int r = i / size, c = i % size, b = (r / boxSize) * boxSize + c / boxSize;

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

    private static bool SolveRecursive(int[] grid, int[] rows, int[] cols, int[] boxes, int size, int boxSize, int fullMask)
    {
        var (index, candidates) = SelectMostConstrainedCell(grid, rows, cols, boxes, size, boxSize, fullMask);

        if (index == -1)
        {
            return true; // No empty cells remain -> solved.
        }

        if (candidates == 0)
        {
            return false; // A cell has no candidates -> dead end.
        }

        int r = index / size, c = index % size, b = (r / boxSize) * boxSize + c / boxSize;

        while (candidates != 0)
        {
            var bit = candidates & -candidates; // lowest set bit
            candidates &= candidates - 1;
            var value = BitOperations.TrailingZeroCount(bit);

            grid[index] = value;
            rows[r] |= bit;
            cols[c] |= bit;
            boxes[b] |= bit;

            if (SolveRecursive(grid, rows, cols, boxes, size, boxSize, fullMask))
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

    private static void CountRecursive(int[] grid, int[] rows, int[] cols, int[] boxes, int size, int boxSize, int fullMask, int cap, ref int count)
    {
        if (count >= cap)
        {
            return;
        }

        var (index, candidates) = SelectMostConstrainedCell(grid, rows, cols, boxes, size, boxSize, fullMask);

        if (index == -1)
        {
            count++;
            return;
        }

        if (candidates == 0)
        {
            return; // dead end
        }

        int r = index / size, c = index % size, b = (r / boxSize) * boxSize + c / boxSize;

        while (candidates != 0 && count < cap)
        {
            var bit = candidates & -candidates;
            candidates &= candidates - 1;
            var value = BitOperations.TrailingZeroCount(bit);

            grid[index] = value;
            rows[r] |= bit;
            cols[c] |= bit;
            boxes[b] |= bit;

            CountRecursive(grid, rows, cols, boxes, size, boxSize, fullMask, cap, ref count);

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
    private static (int Index, int Candidates) SelectMostConstrainedCell(int[] grid, int[] rows, int[] cols, int[] boxes, int size, int boxSize, int fullMask)
    {
        var bestIndex = -1;
        var bestCandidates = 0;
        var bestCount = int.MaxValue;

        for (var i = 0; i < grid.Length; i++)
        {
            if (grid[i] != 0)
            {
                continue;
            }

            int r = i / size, c = i % size, b = (r / boxSize) * boxSize + c / boxSize;
            var candidates = fullMask & ~(rows[r] | cols[c] | boxes[b]);
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
