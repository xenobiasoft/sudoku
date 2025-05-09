using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers;

public static class PuzzleStateExtensions
{
    public static PuzzleState AssertAreEquivalent(this PuzzleState gameState, PuzzleState other)
    {
        gameState.PuzzleId.Should().Be(other.PuzzleId);
        gameState.Board.Count(cell => cell.Value.HasValue).Should().BeInRange(30, 58);

        return gameState;
    }

}