using BenchmarkDotNet.Attributes;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Benchmarks;

/// <summary>
/// Head-to-head generation speed/allocations: legacy <c>PuzzleGenerator</c> (baseline) vs
/// the uniqueness-guaranteeing <c>UniqueSolutionPuzzleGenerator</c>, across all difficulties.
/// </summary>
[MemoryDiagnoser]
public class GeneratorBenchmarks
{
    private IPuzzleGenerator _oldGenerator = null!;
    private IPuzzleGenerator _newGenerator = null!;

    [Params("Easy", "Medium", "Hard", "Expert")]
    public string Difficulty { get; set; } = "Easy";

    private GameDifficulty SelectedDifficulty => GameDifficulty.FromName(Difficulty);

    [GlobalSetup]
    public void Setup()
    {
        _oldGenerator = AlgorithmFactory.CreateOldGenerator();
        _newGenerator = AlgorithmFactory.CreateNewGenerator();
    }

    [Benchmark(Baseline = true)]
    public Task<SudokuPuzzle> OldGenerator() => _oldGenerator.GeneratePuzzleAsync(SelectedDifficulty, BoardSize.Nine);

    [Benchmark]
    public Task<SudokuPuzzle> NewGenerator() => _newGenerator.GeneratePuzzleAsync(SelectedDifficulty, BoardSize.Nine);
}
