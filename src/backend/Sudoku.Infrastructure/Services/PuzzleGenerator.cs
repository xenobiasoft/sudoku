using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Utilities;

namespace Sudoku.Infrastructure.Services;

public class PuzzleGenerator(IPuzzleSolver puzzleSolver) : IPuzzleGenerator
{
    public async Task<SudokuPuzzle> GeneratePuzzleAsync(GameDifficulty difficulty)
    {
        var puzzle = await GenerateEmptyPuzzleAsync().ConfigureAwait(false);

        try
        {
            puzzle = await puzzleSolver.SolvePuzzle(puzzle).ConfigureAwait(false);
        }
        catch (InvalidPuzzleException)
        {
            return await GeneratePuzzleAsync(difficulty);
        }

        puzzle = CreateEmptyCells(puzzle, difficulty);

        puzzle = LockCompletedCells(puzzle);

        return puzzle;
    }

    private async Task<SudokuPuzzle> GenerateEmptyPuzzleAsync()
    {
        var cells = Enumerable.Range(0, 81)
            .Select(i => Cell.CreateEmpty(i / 9, i % 9))
            .ToList();
        var puzzle = SudokuPuzzle.Create(GameId.New(), GameDifficulty.Easy, cells);
        return await Task.FromResult(puzzle);
    }

    private SudokuPuzzle CreateEmptyCells(SudokuPuzzle puzzle, GameDifficulty difficulty)
    {
        var numberOfEmptyCells = difficulty.Name switch
        {
            "Easy" => RandomGenerator.RandomNumber(40, 45),
            "Medium" => RandomGenerator.RandomNumber(46, 49),
            "Hard" => RandomGenerator.RandomNumber(50, 53),
            "Expert" => RandomGenerator.RandomNumber(54, 58),
            _ => 0
        };
        var emptyCellCoords = new List<(int Row, int Col)>();

        while (emptyCellCoords.Count < numberOfEmptyCells)
        {
            var emptyCells = GetRandomCellCoordinates();
            if (!emptyCellCoords.Contains(emptyCells[0]) && emptyCellCoords.Count < numberOfEmptyCells)
            {
                emptyCellCoords.Add(emptyCells[0]);
            }
            if (!emptyCellCoords.Contains(emptyCells[1]) && emptyCellCoords.Count < numberOfEmptyCells)
            {
                emptyCellCoords.Add(emptyCells[1]);
            }
        }

        foreach (var rowCol in emptyCellCoords)
        {
            puzzle.GetCell(rowCol.Row, rowCol.Col).SetValue(null);
            puzzle.GetCell(rowCol.Row, rowCol.Row).SetValue(null);

            if (puzzle.Cells.Count(x => !x.Value.HasValue) >= numberOfEmptyCells) break;
        }

        return puzzle;
    }

    private SudokuPuzzle LockCompletedCells(SudokuPuzzle puzzle)
    {
        var cells = puzzle.Cells.Select(cell => cell.Value.HasValue
                ? Cell.CreateFixed(cell.Row, cell.Column, cell.Value.Value)
                : Cell.CreateEmpty(cell.Row, cell.Column))
            .ToList();

        return SudokuPuzzle.Create(puzzle.PuzzleId, puzzle.Difficulty, cells);
    }

    private (int Row, int Col)[] GetRandomCellCoordinates()
    {
        var randomCells = new List<(int Row, int Col)>();
        var row = RandomGenerator.RandomNumber(0, 9);
        var col = RandomGenerator.RandomNumber(0, 9);

        randomCells.Add((row, col));
        randomCells.Add((8 - row, 8 - col));

        return randomCells.ToArray();
    }
}