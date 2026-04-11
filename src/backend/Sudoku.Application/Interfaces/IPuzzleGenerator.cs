using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Interfaces;

public interface IPuzzleGenerator
{
    Task<SudokuPuzzle> GeneratePuzzleAsync(GameDifficulty difficulty);
}