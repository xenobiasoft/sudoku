using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers;

public static class GameStateMemoryExtensions
{
    public static GameStateMemory VerifyNewGameState(this GameStateMemory gameState)
    {
        gameState.PuzzleId.IsGuid().Should().BeTrue();
        gameState.LastUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        gameState.Board.Should().NotBeNullOrEmpty();
        gameState.Board.Count(cell => cell.Value.HasValue).Should().BeInRange(30, 58);

        return gameState;
    }
}
