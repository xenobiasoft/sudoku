# Sudoku.Benchmarks

A/B harness for comparing the **legacy** Sudoku algorithms against the **new** fast
implementations — both for raw speed (via [BenchmarkDotNet](https://benchmarkdotnet.org/))
and for correctness quality (a unique-solution validation report).

| Concern | Legacy (baseline) | New |
| --- | --- | --- |
| Solve | `StrategyBasedPuzzleSolver` | `BitwiseBacktrackingPuzzleSolver` |
| Generate | `PuzzleGenerator` | `UniqueSolutionPuzzleGenerator` |

The new solver is a bitmask + MRV backtracking engine (`BitwiseSolverEngine`); the new
generator uses that engine to **guarantee every puzzle has exactly one solution** — something
the legacy generator never verifies.

## Prerequisites

- .NET 10 SDK
- Run in **Release** — BenchmarkDotNet refuses to measure a Debug build.

## Speed benchmarks (BenchmarkDotNet)

Run from the repo root. Pass a filter so you don't run every benchmark at once:

```bash
# Generator: legacy vs new, across Easy/Medium/Hard/Expert
dotnet run -c Release --project src/backend/Sudoku.Benchmarks -- --filter *GeneratorBenchmarks*

# Solver: legacy vs new, over fixed seed puzzles (Easy, Medium)
dotnet run -c Release --project src/backend/Sudoku.Benchmarks -- --filter *SolverBenchmarks*

# Everything
dotnet run -c Release --project src/backend/Sudoku.Benchmarks -- --filter *
```

Useful flags (forwarded straight to BenchmarkDotNet):

- `--job short` — fewer warmup/iteration cycles for a quick (noisier) read.
- `--list flat` — list available benchmarks without running them.

Each run prints a summary table (Mean, Error, StdDev, allocations) with the legacy method
marked as the baseline and a `Ratio` column showing the new method's relative speed. Full
reports are written to `BenchmarkDotNet.Artifacts/`.

### What's measured

- **`GeneratorBenchmarks`** — `[Params]` over the four difficulties; `OldGenerator` (baseline)
  vs `NewGenerator`. `[MemoryDiagnoser]` reports allocations.
- **`SolverBenchmarks`** — `[ParamsSource]` over the head-to-head seed puzzles in
  `SeedPuzzles.HeadToHead`; `OldSolver` (baseline) vs `NewSolver`. A fresh puzzle is parsed per
  invocation (the legacy solver mutates its input), so the parse cost is shared and fair.

## Quality / correctness report (`--validate`)

Raw timing can't show the new generator's real win — **guaranteed uniqueness**. The validate
mode generates many puzzles with each generator and reports the unique-solution rate plus the
clue-count distribution:

```bash
# Default: 30 puzzles per difficulty per generator
dotnet run -c Release --project src/backend/Sudoku.Benchmarks -- --validate

# Custom sample size
dotnet run -c Release --project src/backend/Sudoku.Benchmarks -- --validate --samples 50
```

Example shape of the output:

```
Generator                        Difficulty    Unique%  AvgClues  MinClues  MaxClues
--------------------------------------------------------------------------------------
Legacy PuzzleGenerator           Easy            ...%      ...       ...       ...
UniqueSolutionPuzzleGenerator    Easy          100.0%      ...       ...       ...
...
```

`Unique%` is the share of generated puzzles with exactly one solution (verified with
`BitwiseSolverEngine.CountSolutions`). The new generator should report **100%**.

> Tip: don't run `--validate` and the speed benchmarks at the same time — CPU contention
> makes the BenchmarkDotNet numbers unreliable.

## Adding a new algorithm to the comparison

1. Implement `IPuzzleSolver` / `IPuzzleGenerator` (or a new engine) in `Sudoku.Infrastructure`.
2. Add a factory method in `AlgorithmFactory`.
3. Add a `[Benchmark]` method to the relevant benchmark class (keep the legacy one as
   `Baseline = true`), and/or a row in `QualityReport`.

## Files

- `Program.cs` — entry point; routes `--validate` to the quality report, everything else to
  BenchmarkDotNet's `BenchmarkSwitcher`.
- `AlgorithmFactory.cs` — constructs legacy and new implementations without the Azure DI graph.
- `GeneratorBenchmarks.cs` / `SolverBenchmarks.cs` — the head-to-head speed benchmarks.
- `QualityReport.cs` — the `--validate` correctness report.
- `SeedPuzzles.cs` / `PuzzleParser.cs` — fixed input puzzles and an 81-char board parser.
