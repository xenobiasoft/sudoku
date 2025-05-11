using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Generator;

namespace XenobiaSoft.Sudoku;

public class SudokuGame(IPuzzleGenerator puzzleGenerator, IGameStateStorage<GameStateMemory> gameStateStorage)
    : ISudokuGame
{
    public async Task<GameStateMemory> NewGameAsync(Level level)
    {
        var puzzle = await puzzleGenerator.Generate(level).ConfigureAwait(false);

        var gameState = new GameStateMemory(puzzle.PuzzleId, puzzle.GetAllCells())
        {
            InvalidMoves = 0,
            TotalMoves = 0,
            PlayDuration = TimeSpan.Zero,
        };

        return gameState;
    }

    public Task SaveAsync(GameStateMemory memory)
    {
        return gameStateStorage.SaveAsync(memory);
    }

    public Task DeleteAsync(string puzzleId)
    {
        return gameStateStorage.DeleteAsync(puzzleId);
    }

    public Task<GameStateMemory> LoadAsync(string puzzleId)
    {
        return gameStateStorage.LoadAsync(puzzleId);
    }
}