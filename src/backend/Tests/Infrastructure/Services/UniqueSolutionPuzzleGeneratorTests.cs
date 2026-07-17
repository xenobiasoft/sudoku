using DepenMock.Attributes;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Services;
using Sudoku.Infrastructure.Services.Solvers;

namespace UnitTests.Infrastructure.Services;

[LogOutput(LogOutputTiming.Always)]
public class UniqueSolutionPuzzleGeneratorTests : MoqBaseTestByAbstraction<UniqueSolutionPuzzleGenerator, IPuzzleGenerator>
{
    public static IEnumerable<object[]> Difficulties =>
    [
        [GameDifficulty.Easy],
        [GameDifficulty.Medium],
        [GameDifficulty.Hard],
        [GameDifficulty.Expert],
    ];

    [Theory]
    [MemberData(nameof(Difficulties))]
    public async Task GeneratePuzzleAsync_ForAnyDifficulty_ProducesAValidPuzzle(GameDifficulty difficulty)
    {
        var sut = ResolveSut();

        var puzzle = await sut.GeneratePuzzleAsync(difficulty);

        puzzle.Should().NotBeNull();
        puzzle.IsValid().Should().BeTrue();
        puzzle.Difficulty.Should().Be(difficulty);
    }

    [Theory]
    [MemberData(nameof(Difficulties))]
    public async Task GeneratePuzzleAsync_ForAnyDifficulty_ProducesExactlyOneSolution(GameDifficulty difficulty)
    {
        var sut = ResolveSut();

        var puzzle = await sut.GeneratePuzzleAsync(difficulty);

        var grid = BitwiseSolverGridMapper.ToGrid(puzzle);
        BitwiseSolverEngine.CountSolutions(grid, cap: 2).Should().Be(1);
    }

    [Theory]
    [MemberData(nameof(Difficulties))]
    public async Task GeneratePuzzleAsync_RemainingCluesAreFixed(GameDifficulty difficulty)
    {
        var sut = ResolveSut();

        var puzzle = await sut.GeneratePuzzleAsync(difficulty);

        puzzle.Cells.Where(c => c.HasValue).Should().OnlyContain(c => c.IsFixed);
        puzzle.Cells.Where(c => !c.HasValue).Should().OnlyContain(c => !c.IsFixed);
    }

    [Theory]
    [MemberData(nameof(Difficulties))]
    public async Task GeneratePuzzleAsync_ClueCountStaysWithinDifficultyBand(GameDifficulty difficulty)
    {
        var sut = ResolveSut();

        var puzzle = await sut.GeneratePuzzleAsync(difficulty);

        var emptyCells = puzzle.GetEmptyCellCount();
        var (min, max) = ExpectedEmptyBand(difficulty);

        emptyCells.Should().BeLessThanOrEqualTo(max);
        emptyCells.Should().BeGreaterThanOrEqualTo(min - LowerBandMargin);
    }

    [Fact]
    public async Task GeneratePuzzleAsync_Expert_DigsPastTheHardBand()
    {
        var sut = ResolveSut();
        var (expertFloor, _) = ExpectedEmptyBand(GameDifficulty.Expert);

        var emptyCells = await GenerateEmptyCellCounts(sut, GameDifficulty.Expert);

        // Regression guard. A symmetric-only dig stalls around 50 empty cells, which left
        // Expert sitting inside Hard's band (50-53) — the player got a Hard puzzle wearing
        // an Expert label. Note "Expert digs deeper than Hard on average" would NOT catch
        // that (it was already true while the bug was live); only clearing the band floor
        // does. Averaged over a sample so the rare one-cell shortfall can't flake the run.
        emptyCells.Average().Should().BeGreaterThanOrEqualTo(expertFloor);
    }

    private static async Task<List<int>> GenerateEmptyCellCounts(IPuzzleGenerator generator, GameDifficulty difficulty)
    {
        var counts = new List<int>();

        for (var i = 0; i < SampleSize; i++)
        {
            var puzzle = await generator.GeneratePuzzleAsync(difficulty);
            counts.Add(puzzle.GetEmptyCellCount());
        }

        return counts;
    }

    private const int SampleSize = 10;

    // Digging stops at the last state that still has a unique solution, which very
    // occasionally lands one cell short of the target floor (measured: ~0.2% of Expert
    // puzzles). This margin absorbs that without letting a genuinely under-dug puzzle pass.
    private const int LowerBandMargin = 1;

    private static (int Min, int Max) ExpectedEmptyBand(GameDifficulty difficulty) => difficulty.Name switch
    {
        "Easy" => (40, 45),
        "Medium" => (46, 49),
        "Hard" => (50, 53),
        "Expert" => (54, 58),
        _ => (0, 81)
    };
}
