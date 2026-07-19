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

        var puzzle = await sut.GeneratePuzzleAsync(difficulty, BoardSize.Nine);

        puzzle.Should().NotBeNull();
        puzzle.IsValid().Should().BeTrue();
        puzzle.Difficulty.Should().Be(difficulty);
    }

    [Theory]
    [MemberData(nameof(Difficulties))]
    public async Task GeneratePuzzleAsync_ForAnyDifficulty_ProducesExactlyOneSolution(GameDifficulty difficulty)
    {
        var sut = ResolveSut();

        var puzzle = await sut.GeneratePuzzleAsync(difficulty, BoardSize.Nine);

        var grid = BitwiseSolverGridMapper.ToGrid(puzzle);
        BitwiseSolverEngine.CountSolutions(grid, cap: 2).Should().Be(1);
    }

    [Theory]
    [MemberData(nameof(Difficulties))]
    public async Task GeneratePuzzleAsync_RemainingCluesAreFixed(GameDifficulty difficulty)
    {
        var sut = ResolveSut();

        var puzzle = await sut.GeneratePuzzleAsync(difficulty, BoardSize.Nine);

        puzzle.Cells.Where(c => c.HasValue).Should().OnlyContain(c => c.IsFixed);
        puzzle.Cells.Where(c => !c.HasValue).Should().OnlyContain(c => !c.IsFixed);
    }

    [Theory]
    [MemberData(nameof(Difficulties))]
    public async Task GeneratePuzzleAsync_ClueCountStaysWithinDifficultyBand(GameDifficulty difficulty)
    {
        var sut = ResolveSut();

        var puzzle = await sut.GeneratePuzzleAsync(difficulty, BoardSize.Nine);

        var emptyCells = puzzle.GetEmptyCellCount();
        var (min, max) = ExpectedEmptyBand(difficulty, BoardSize.Nine);

        emptyCells.Should().BeLessThanOrEqualTo(max);
        emptyCells.Should().BeGreaterThanOrEqualTo(min - LowerBandMargin);
    }

    [Fact]
    public async Task GeneratePuzzleAsync_Expert_DigsPastTheHardBand()
    {
        var sut = ResolveSut();
        var (expertFloor, _) = ExpectedEmptyBand(GameDifficulty.Expert, BoardSize.Nine);

        var emptyCells = await GenerateEmptyCellCounts(sut, GameDifficulty.Expert, BoardSize.Nine, NineSampleSize);

        // Regression guard. A symmetric-only dig stalls around 50 empty cells, which left
        // Expert sitting inside Hard's band (50-53) — the player got a Hard puzzle wearing
        // an Expert label. Note "Expert digs deeper than Hard on average" would NOT catch
        // that (it was already true while the bug was live); only clearing the band floor
        // does. Averaged over a sample so the rare one-cell shortfall can't flake the run.
        emptyCells.Average().Should().BeGreaterThanOrEqualTo(expertFloor);
    }

    // --- 16x16 coverage -------------------------------------------------------------
    //
    // Generation at 16x16 is inherently far more expensive than 9x9 (256 cells vs 81), and
    // gets steeply more expensive as the empty-cell target grows (Phase 2 measurement: see
    // UniqueSolutionPuzzleGenerator's remarks). Easy is fast enough to run unconditionally
    // as a smoke test; Medium/Hard/Expert are tagged SlowGeneration and excluded from the
    // default CI run (`--filter Category!=SlowGeneration`) so the fast unit-test suite stays
    // fast, per the spec's testing-strategy guidance. Run them on demand or nightly.

    [Fact]
    public async Task GeneratePuzzleAsync_SixteenEasy_ProducesAValidUniqueSolutionPuzzleQuickly()
    {
        var sut = ResolveSut();

        var puzzle = await sut.GeneratePuzzleAsync(GameDifficulty.Easy, BoardSize.Sixteen);

        puzzle.Should().NotBeNull();
        puzzle.IsValid().Should().BeTrue();
        puzzle.Difficulty.Should().Be(GameDifficulty.Easy);
        var grid = BitwiseSolverGridMapper.ToGrid(puzzle);
        BitwiseSolverEngine.CountSolutions(grid, cap: 2).Should().Be(1);
        var (min, max) = ExpectedEmptyBand(GameDifficulty.Easy, BoardSize.Sixteen);
        puzzle.GetEmptyCellCount().Should().BeInRange(min - LowerBandMargin, max);
    }

    [Theory]
    [Trait("Category", "SlowGeneration")]
    [MemberData(nameof(Difficulties))]
    public async Task GeneratePuzzleAsync_ForAnySixteenDifficulty_ProducesAValidPuzzle(GameDifficulty difficulty)
    {
        var sut = ResolveSut();

        var puzzle = await sut.GeneratePuzzleAsync(difficulty, BoardSize.Sixteen);

        puzzle.Should().NotBeNull();
        puzzle.IsValid().Should().BeTrue();
        puzzle.Difficulty.Should().Be(difficulty);
    }

    [Theory]
    [Trait("Category", "SlowGeneration")]
    [MemberData(nameof(Difficulties))]
    public async Task GeneratePuzzleAsync_ForAnySixteenDifficulty_ProducesExactlyOneSolution(GameDifficulty difficulty)
    {
        var sut = ResolveSut();

        var puzzle = await sut.GeneratePuzzleAsync(difficulty, BoardSize.Sixteen);

        var grid = BitwiseSolverGridMapper.ToGrid(puzzle);
        BitwiseSolverEngine.CountSolutions(grid, cap: 2).Should().Be(1);
    }

    [Theory]
    [Trait("Category", "SlowGeneration")]
    [MemberData(nameof(Difficulties))]
    public async Task GeneratePuzzleAsync_ForAnySixteenDifficulty_RemainingCluesAreFixed(GameDifficulty difficulty)
    {
        var sut = ResolveSut();

        var puzzle = await sut.GeneratePuzzleAsync(difficulty, BoardSize.Sixteen);

        puzzle.Cells.Where(c => c.HasValue).Should().OnlyContain(c => c.IsFixed);
        puzzle.Cells.Where(c => !c.HasValue).Should().OnlyContain(c => !c.IsFixed);
    }

    [Theory]
    [Trait("Category", "SlowGeneration")]
    [MemberData(nameof(Difficulties))]
    public async Task GeneratePuzzleAsync_ForAnySixteenDifficulty_ClueCountStaysWithinDifficultyBand(GameDifficulty difficulty)
    {
        var sut = ResolveSut();

        var puzzle = await sut.GeneratePuzzleAsync(difficulty, BoardSize.Sixteen);

        var emptyCells = puzzle.GetEmptyCellCount();
        var (min, max) = ExpectedEmptyBand(difficulty, BoardSize.Sixteen);

        emptyCells.Should().BeLessThanOrEqualTo(max);
        emptyCells.Should().BeGreaterThanOrEqualTo(min - LowerBandMargin);
    }

    private static async Task<List<int>> GenerateEmptyCellCounts(IPuzzleGenerator generator, GameDifficulty difficulty, BoardSize size, int sampleSize)
    {
        var counts = new List<int>();

        for (var i = 0; i < sampleSize; i++)
        {
            var puzzle = await generator.GeneratePuzzleAsync(difficulty, size);
            counts.Add(puzzle.GetEmptyCellCount());
        }

        return counts;
    }

    private const int NineSampleSize = 10;

    // Digging stops at the last state that still has a unique solution, which very
    // occasionally lands one cell short of the target floor (measured: ~0.2% of Expert
    // puzzles). This margin absorbs that without letting a genuinely under-dug puzzle pass.
    private const int LowerBandMargin = 1;

    private static (int Min, int Max) ExpectedEmptyBand(GameDifficulty difficulty, BoardSize size)
    {
        if (size == BoardSize.Sixteen)
        {
            return difficulty.Name switch
            {
                "Easy" => (126, 142),
                "Medium" => (145, 155),
                "Hard" => (156, 162),
                "Expert" => (163, 170),
                _ => (0, size.CellCount)
            };
        }

        return difficulty.Name switch
        {
            "Easy" => (40, 45),
            "Medium" => (46, 49),
            "Hard" => (50, 53),
            "Expert" => (54, 58),
            _ => (0, size.CellCount)
        };
    }
}
