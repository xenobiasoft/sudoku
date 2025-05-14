using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku.Extensions;

public static class GameStateMemoryExtensions
{
    public static bool HasSameBoardStateAs(this GameStateMemory gameStateMemory, GameStateMemory gameState)
    {
        return gameStateMemory.PuzzleId == gameState.PuzzleId &&
               gameStateMemory.TotalMoves == gameState.TotalMoves &&
               gameStateMemory.Board.SequenceEqual(gameState.Board);
    }
}