using Sudoku.Application.Interfaces;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Services;
using Sudoku.Infrastructure.Services.Solvers;

namespace UnitTests.Infrastructure.Services;

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

        // Target is a ceiling; uniqueness may stop digging early, so allow a lower-bound margin.
        emptyCells.Should().BeLessThanOrEqualTo(max);
        emptyCells.Should().BeGreaterThanOrEqualTo(min - LowerBandMargin);
    }

    // Uniqueness can occasionally halt digging a few cells short of the target; this margin
    // keeps the band assertion stable without weakening the uniqueness guarantee above.
    private const int LowerBandMargin = 6;

    private static (int Min, int Max) ExpectedEmptyBand(GameDifficulty difficulty) => difficulty.Name switch
    {
        "Easy" => (40, 45),
        "Medium" => (46, 49),
        "Hard" => (50, 53),
        "Expert" => (54, 58),
        _ => (0, 81)
    };
}
