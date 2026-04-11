using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Utilities;

namespace Sudoku.Infrastructure.Repositories;

public class InMemoryPuzzleRepository : IPuzzleRepository
{
	private readonly CircularStack<SudokuPuzzle> _gameState = new(100);
    
    public Task<SudokuPuzzle> CreateAsync(string alias, GameDifficulty difficulty)
    {
        throw new NotSupportedException();
    }

    public Task DeleteAsync(string alias, string puzzleId)
    {
        _gameState.Clear();

        return Task.CompletedTask;
    }

    public Task<SudokuPuzzle> LoadAsync(string alias, string puzzleId)
    {
        return Task.FromResult(_gameState.Count > 0 ? _gameState.Peek() : null);
    }

    public Task<IEnumerable<SudokuPuzzle>> LoadAllAsync(string alias)
    {
        throw new NotSupportedException();
    }

    public async Task<SudokuPuzzle> ResetAsync(string alias, string puzzleId)
    {
        while (_gameState.Count > 1)
        {
            await UndoAsync(alias, puzzleId);
        }

        return _gameState.Peek();
    }

    public Task SaveAsync(SudokuPuzzle gameState)
    {
        if (_gameState.Count > 0)
        {
            var previousGameState = _gameState.Peek();

            if (AreGameStatesEqual(gameState, previousGameState)) return Task.CompletedTask;
        }

        _gameState.Push(gameState);

        return Task.CompletedTask;
    }

    public Task<SudokuPuzzle> UndoAsync(string alias, string puzzleId)
    {
        return _gameState.Count == 0 ? Task.FromResult<SudokuPuzzle>(null) : Task.FromResult(_gameState.Pop());
    }

    private bool AreGameStatesEqual(SudokuPuzzle gameState1, SudokuPuzzle gameState2)
    {
        if (gameState1.PuzzleId != gameState2.PuzzleId)
        {
            return false;
        }

        return AreBoardsEqual(gameState1.Cells, gameState2.Cells);
    }

    private bool AreBoardsEqual(IEnumerable<Cell> board1, IEnumerable<Cell> board2)
    {
        var cells1 = board1.ToDictionary(c => (c.Row, c.Column), c => c);
        var cells2 = board2.ToDictionary(c => (c.Row, c.Column), c => c);
        
        if (cells1.Count != cells2.Count)
        {
            return false;
        }

        foreach (var cell in cells1)
        {
            if (!cells2.TryGetValue(cell.Key, out var cell2))
            {
                return false;
            }

            var cell1 = cell.Value;

            if (cell1.Value != cell2.Value)
            {
                return false;
            }

            if (cell1.PossibleValues.Count != cell2.PossibleValues.Count)
            {
                return false;
            }

            if (!cell1.PossibleValues.SetEquals(cell2.PossibleValues))
            {
                return false;
            }
        }

        return true;
    }
}