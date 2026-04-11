using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Interfaces;

public interface IPuzzleRepository
{
    Task<SudokuPuzzle> CreateAsync(string alias, GameDifficulty difficulty);
    Task DeleteAsync(string alias, string puzzleId);
    Task<SudokuPuzzle> LoadAsync(string alias, string puzzleId);
    Task<IEnumerable<SudokuPuzzle>> LoadAllAsync(string alias);
    Task<SudokuPuzzle> ResetAsync(string alias, string puzzleId);
    Task SaveAsync(SudokuPuzzle gameState);
    Task<SudokuPuzzle> UndoAsync(string alias, string puzzleId);
}