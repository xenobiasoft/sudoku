using XenobiaSoft.Sudoku.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku;

[Obsolete("Use Sudoku.Domain.SudokuGame instead")]
public class SudokuGame(IPuzzleGenerator puzzleGenerator, IGameStateStorage gameStateStorage) : ISudokuGame
{
    public async Task<GameStateMemory> NewGameAsync(string alias, GameDifficulty difficulty)
    {
        var puzzle = await puzzleGenerator.GenerateAsync(difficulty).ConfigureAwait(false);

        var gameState = new GameStateMemory
        {
            Alias = alias,
            Board = puzzle.GetAllCells(),
            InvalidMoves = 0,
            PlayDuration = TimeSpan.Zero,
            PuzzleId = puzzle.PuzzleId,
            TotalMoves = 0,
        };

        return gameState;
    }

    public Task SaveAsync(GameStateMemory memory)
    {
        return gameStateStorage.SaveAsync(memory);
    }

    public Task DeleteAsync(string alias, string puzzleId)
    {
        return gameStateStorage.DeleteAsync(alias, puzzleId);
    }

    public Task<GameStateMemory> LoadAsync(string alias, string puzzleId)
    {
        return gameStateStorage.LoadAsync(alias, puzzleId);
    }
}