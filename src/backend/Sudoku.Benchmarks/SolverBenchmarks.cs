using BenchmarkDotNet.Attributes;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Benchmarks;

/// <summary>
/// Head-to-head solve speed/allocations: legacy <c>StrategyBasedPuzzleSolver</c> (baseline)
/// vs the <c>BitwiseBacktrackingPuzzleSolver</c>, over identical seed puzzles. Each benchmark
/// parses a fresh puzzle per invocation (the legacy solver mutates its input), so the parse
/// cost is shared and the comparison stays fair.
/// </summary>
[MemoryDiagnoser]
public class SolverBenchmarks
{
    private IPuzzleSolver _oldSolver = null!;
    private IPuzzleSolver _newSolver = null!;
    private string _board = SeedPuzzles.Easy;

    [ParamsSource(nameof(PuzzleNames))]
    public string Puzzle { get; set; } = "Easy";

    public IEnumerable<string> PuzzleNames => SeedPuzzles.HeadToHead.Keys;

    [GlobalSetup]
    public void Setup()
    {
        _oldSolver = AlgorithmFactory.CreateOldSolver();
        _newSolver = AlgorithmFactory.CreateNewSolver();
        _board = SeedPuzzles.HeadToHead[Puzzle];
    }

    [Benchmark(Baseline = true)]
    public Task<SudokuPuzzle> OldSolver() =>
        _oldSolver.SolvePuzzle(PuzzleParser.Parse(_board, GameDifficulty.Hard));

    [Benchmark]
    public Task<SudokuPuzzle> NewSolver() =>
        _newSolver.SolvePuzzle(PuzzleParser.Parse(_board, GameDifficulty.Hard));
}
