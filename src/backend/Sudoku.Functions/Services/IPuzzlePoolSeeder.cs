namespace Sudoku.Functions.Services;

public interface IPuzzlePoolSeeder
{
    /// <summary>
    /// Tops the pools back up to their target size.
    /// Returns the total number of puzzles seeded across every combination that ran.
    /// </summary>
    /// <param name="filter">
    /// Optionally narrows the run to a single board size and/or difficulty and overrides the
    /// target pool size. Null tops up the whole supported matrix.
    /// </param>
    Task<int> SeedPoolAsync(PuzzlePoolSeedFilter? filter = null);
}
