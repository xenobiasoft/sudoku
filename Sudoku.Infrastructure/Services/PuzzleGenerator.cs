using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace XenobiaSoft.Sudoku.Infrastructure.Services;

public class PuzzleGenerator(IPuzzleSolver puzzleSolver) : IPuzzleGenerator
{
    public Task<SudokuPuzzle> GeneratePuzzleAsync(GameDifficulty difficulty)
    {
        throw new NotImplementedException();
    }
}