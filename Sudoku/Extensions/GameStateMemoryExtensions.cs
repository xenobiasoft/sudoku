using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku.Extensions;

public static class GameStateMemoryExtensions
{
    public static bool IsSameGameStateAs(this GameStateMemory gameState1, GameStateMemory gameState2)
    {
        return gameState1.PuzzleId == gameState2.PuzzleId && gameState1.HasSameBoardStateAs(gameState2);
    }

    public static bool HasSameBoardStateAs(this GameStateMemory gameState1, GameStateMemory gameState2)
    {
        if (gameState1.Board.Length != gameState2.Board.Length) return false;

        for (var i = 0; i < gameState1.Board.Length; i++)
        {
            if (gameState1.Board[i].Row != gameState2.Board[i].Row ||
                gameState1.Board[i].Column != gameState2.Board[i].Column ||
                gameState1.Board[i].Value != gameState2.Board[i].Value ||
                gameState1.Board[i].Locked != gameState2.Board[i].Locked ||
                !gameState1.Board[i].PossibleValues.SequenceEqual(gameState2.Board[i].PossibleValues))
            {
                return false;
            }
        }
        return true;
    }
}