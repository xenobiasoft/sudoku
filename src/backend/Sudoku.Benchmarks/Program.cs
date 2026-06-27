using System.Reflection;
using BenchmarkDotNet.Running;
using Sudoku.Benchmarks;

// `--validate` runs the correctness/quality report (unique-solution rate, clue distribution).
// Anything else is forwarded to BenchmarkDotNet's switcher, e.g.:
//   dotnet run -c Release -- --filter *GeneratorBenchmarks*
//   dotnet run -c Release -- --filter *SolverBenchmarks*
if (args.Contains("--validate"))
{
    var samples = 30;
    var index = Array.IndexOf(args, "--samples");
    if (index >= 0 && index + 1 < args.Length && int.TryParse(args[index + 1], out var parsed))
    {
        samples = parsed;
    }

    QualityReport.Run(samples);
    return;
}

BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);
