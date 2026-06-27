using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Services.Solvers;
using Sudoku.Infrastructure.Utilities;

namespace Sudoku.Infrastructure.Services;

/// <summary>
/// An <see cref="IPuzzleGenerator"/> that guarantees every generated puzzle has exactly
/// one solution. It builds a complete grid with <see cref="BitwiseSolverEngine"/>, then
/// digs holes symmetrically, keeping a removal only while the puzzle still has a unique
/// solution (verified via <see cref="BitwiseSolverEngine.CountSolutions"/>).
///
/// Empty-cell targets per difficulty mirror the legacy <see cref="PuzzleGenerator"/> so the
/// two are directly comparable. The target is a ceiling: if uniqueness can't be preserved
/// all the way to it, generation stops at the last unique state.
/// </summary>
public class UniqueSolutionPuzzleGenerator : IPuzzleGenerator
{
    private const int EASY_EMPTY_MIN = 40;
    private const int EASY_EMPTY_MAX = 45;
    private const int MEDIUM_EMPTY_MIN = 46;
    private const int MEDIUM_EMPTY_MAX = 49;
    private const int HARD_EMPTY_MIN = 50;
    private const int HARD_EMPTY_MAX = 53;
    private const int EXPERT_EMPTY_MIN = 54;
    private const int EXPERT_EMPTY_MAX = 58;

    public Task<SudokuPuzzle> GeneratePuzzleAsync(GameDifficulty difficulty)
    {
        var grid = GenerateFullSolution();
        var targetEmpty = GetTargetEmptyCount(difficulty);

        DigHoles(grid, targetEmpty);

        return Task.FromResult(BuildPuzzle(grid, difficulty));
    }

    private static int[] GenerateFullSolution()
    {
        var grid = new int[81];

        // Seeding the three diagonal boxes is always conflict-free (they share no row,
        // column, or box), and the random fill is what gives each generated puzzle a
        // different solution once the solver completes the rest.
        for (var box = 0; box < 3; box++)
        {
            var values = ShuffledOneToNine();
            var k = 0;

            for (var r = box * 3; r < box * 3 + 3; r++)
            {
                for (var c = box * 3; c < box * 3 + 3; c++)
                {
                    grid[r * 9 + c] = values[k++];
                }
            }
        }

        BitwiseSolverEngine.Solve(grid);

        return grid;
    }

    private static void DigHoles(int[] grid, int targetEmpty)
    {
        var positions = ShuffledPositions();
        var removed = 0;

        foreach (var index in positions)
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

            var mirror = 80 - index;

            // Remove a symmetric pair when there's room for two; otherwise a single cell.
            // Bounding the group by 'remaining' guarantees we never exceed the target.
            var group = (index == mirror || grid[mirror] == 0 || remaining < 2)
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
    }

    private static SudokuPuzzle BuildPuzzle(int[] grid, GameDifficulty difficulty)
    {
        var cells = Enumerable.Range(0, 81)
            .Select(i =>
            {
                int row = i / 9, column = i % 9;
                return grid[i] == 0
                    ? Cell.CreateEmpty(row, column)
                    : Cell.CreateFixed(row, column, grid[i]);
            })
            .ToList();

        return SudokuPuzzle.Create(GameId.New(), difficulty, cells);
    }

    private static int GetTargetEmptyCount(GameDifficulty difficulty) => difficulty.Name switch
    {
        "Easy" => RandomGenerator.RandomNumber(EASY_EMPTY_MIN, EASY_EMPTY_MAX),
        "Medium" => RandomGenerator.RandomNumber(MEDIUM_EMPTY_MIN, MEDIUM_EMPTY_MAX),
        "Hard" => RandomGenerator.RandomNumber(HARD_EMPTY_MIN, HARD_EMPTY_MAX),
        "Expert" => RandomGenerator.RandomNumber(EXPERT_EMPTY_MIN, EXPERT_EMPTY_MAX),
        _ => 0
    };

    private static int[] ShuffledOneToNine()
    {
        var values = Enumerable.Range(1, 9).ToArray();
        Shuffle(values);
        return values;
    }

    private static int[] ShuffledPositions()
    {
        var positions = Enumerable.Range(0, 81).ToArray();
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
