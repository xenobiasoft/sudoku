namespace Sudoku.Benchmarks;

/// <summary>
/// Fixed, well-known unique puzzles so old and new solvers run against identical input.
/// '0' denotes a blank cell.
/// </summary>
public static class SeedPuzzles
{
    // Canonical Wikipedia example puzzle.
    public const string Easy =
        "530070000600195000098000060800060003400803001700020006060000280000419005000080079";

    // A unique medium-difficulty puzzle requiring some guessing.
    public const string Medium =
        "000260701680070090190004500820100040004602900050003028009300074040050036703018000";

    // Arto Inkala's "world's hardest" puzzle — unique, brutal for naive strategies.
    public const string Hardest =
        "800000000003600000070090200050007000000045700000100030001000068008500010090000400";

    /// <summary>Puzzles used for the old-vs-new head-to-head (both solvers handle these).</summary>
    public static readonly IReadOnlyDictionary<string, string> HeadToHead = new Dictionary<string, string>
    {
        ["Easy"] = Easy,
        ["Medium"] = Medium,
    };
}
