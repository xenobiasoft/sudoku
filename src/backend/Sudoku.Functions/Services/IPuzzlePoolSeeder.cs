namespace Sudoku.Functions.Services;

public interface IPuzzlePoolSeeder
{
    /// <summary>
    /// Tops every difficulty's pool back up to the target size.
    /// Returns the total number of puzzles seeded across all difficulties.
    /// </summary>
    Task<int> SeedPoolAsync();
}
