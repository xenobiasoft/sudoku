using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Services.Solvers;

namespace Sudoku.Benchmarks;

/// <summary>
/// "Other stuff" beyond raw timing: generates many puzzles with each algorithm and reports
/// the metrics that prove correctness rather than speed — most importantly the
/// <b>unique-solution rate</b>, plus the clue-count distribution.
/// </summary>
public static class QualityReport
{
    public static void Run(int samplesPerDifficulty)
    {
        var oldGenerator = AlgorithmFactory.CreateOldGenerator();
        var newGenerator = AlgorithmFactory.CreateNewGenerator();
        var difficulties = new[] { GameDifficulty.Easy, GameDifficulty.Medium, GameDifficulty.Hard, GameDifficulty.Expert };

        Console.WriteLine();
        Console.WriteLine($"Quality comparison ({samplesPerDifficulty} puzzles per difficulty)");
        Console.WriteLine(new string('-', 86));
        Console.WriteLine($"{"Generator",-32} {"Difficulty",-10} {"Unique%",8} {"AvgClues",9} {"MinClues",9} {"MaxClues",9}");
        Console.WriteLine(new string('-', 86));

        foreach (var difficulty in difficulties)
        {
            ReportRow("Legacy PuzzleGenerator", oldGenerator, difficulty, samplesPerDifficulty);
            ReportRow("UniqueSolutionPuzzleGenerator", newGenerator, difficulty, samplesPerDifficulty);
        }

        Console.WriteLine(new string('-', 86));
        Console.WriteLine("Unique% = share of generated puzzles with exactly one solution (the correctness win).");
    }

    private static void ReportRow(string name, IPuzzleGenerator generator, GameDifficulty difficulty, int samples)
    {
        var uniqueCount = 0;
        var clueCounts = new List<int>(samples);

        for (var i = 0; i < samples; i++)
        {
            var puzzle = generator.GeneratePuzzleAsync(difficulty).GetAwaiter().GetResult();
            var grid = BitwiseSolverGridMapper.ToGrid(puzzle);

            if (BitwiseSolverEngine.CountSolutions(grid, cap: 2) == 1)
            {
                uniqueCount++;
            }

            clueCounts.Add(CountClues(puzzle));
        }

        var uniquePct = 100.0 * uniqueCount / samples;
        Console.WriteLine(
            $"{name,-32} {difficulty.Name,-10} {uniquePct,7:0.0}% {clueCounts.Average(),9:0.0} {clueCounts.Min(),9} {clueCounts.Max(),9}");
    }

    private static int CountClues(SudokuPuzzle puzzle) => puzzle.Cells.Count(c => c.HasValue);
}
