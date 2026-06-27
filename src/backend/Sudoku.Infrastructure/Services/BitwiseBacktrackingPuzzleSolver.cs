using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Infrastructure.Services.Solvers;

namespace Sudoku.Infrastructure.Services;

/// <summary>
/// An <see cref="IPuzzleSolver"/> backed by the fast <see cref="BitwiseSolverEngine"/>.
/// Drop-in alternative to <see cref="StrategyBasedPuzzleSolver"/> with no repository,
/// async round-trips, or random guessing in the solve path.
/// </summary>
public class BitwiseBacktrackingPuzzleSolver : IPuzzleSolver
{
    public Task<SudokuPuzzle> SolvePuzzle(SudokuPuzzle puzzle)
    {
        var grid = BitwiseSolverGridMapper.ToGrid(puzzle);

        BitwiseSolverEngine.Solve(grid);

        return Task.FromResult(BitwiseSolverGridMapper.ApplyTo(puzzle, grid));
    }
}
