using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Utilities;

namespace Sudoku.Infrastructure.Services;

public class PuzzleGenerator(IPuzzleSolver puzzleSolver) : IPuzzleGenerator
{
    private const int EASY_EMPTY_MIN = 40;
    private const int EASY_EMPTY_MAX = 45;
    private const int MEDIUM_EMPTY_MIN = 46;
    private const int MEDIUM_EMPTY_MAX = 49;
    private const int HARD_EMPTY_MIN = 50;
    private const int HARD_EMPTY_MAX = 53;
    private const int EXPERT_EMPTY_MIN = 54;
    private const int EXPERT_EMPTY_MAX = 58;

    public async Task<SudokuPuzzle> GeneratePuzzleAsync(GameDifficulty difficulty, BoardSize size)
    {
        if (size != BoardSize.Nine)
        {
            throw new NotSupportedException(
                $"The legacy {nameof(PuzzleGenerator)} only supports {BoardSize.Nine}; {size} is not supported.");
        }

        var puzzle = await GenerateEmptyPuzzleAsync().ConfigureAwait(false);

        try
        {
            puzzle = await puzzleSolver.SolvePuzzle(puzzle).ConfigureAwait(false);
        }
        catch (InvalidPuzzleException)
        {
            return await GeneratePuzzleAsync(difficulty, size);
        }

        puzzle = CreateEmptyCells(puzzle, difficulty);

        puzzle = LockCompletedCells(puzzle);

        return puzzle;
    }

    private async Task<SudokuPuzzle> GenerateEmptyPuzzleAsync()
    {
        var cells = Enumerable.Range(0, 81)
            .Select(i => Cell.CreateEmpty(i / 9, i % 9, BoardSize.Nine))
            .ToList();
        var puzzle = SudokuPuzzle.Create(GameId.New(), GameDifficulty.Easy, BoardSize.Nine, cells);
        return await Task.FromResult(puzzle);
    }

    private SudokuPuzzle CreateEmptyCells(SudokuPuzzle puzzle, GameDifficulty difficulty)
    {
        var numberOfEmptyCells = difficulty.Name switch
        {
            "Easy" => RandomGenerator.RandomNumber(EASY_EMPTY_MIN, EASY_EMPTY_MAX),
            "Medium" => RandomGenerator.RandomNumber(MEDIUM_EMPTY_MIN, MEDIUM_EMPTY_MAX),
            "Hard" => RandomGenerator.RandomNumber(HARD_EMPTY_MIN, HARD_EMPTY_MAX),
            "Expert" => RandomGenerator.RandomNumber(EXPERT_EMPTY_MIN, EXPERT_EMPTY_MAX),
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

            if (puzzle.Cells.Count(x => !x.Value.HasValue) >= numberOfEmptyCells) break;
        }

        return puzzle;
    }

    private SudokuPuzzle LockCompletedCells(SudokuPuzzle puzzle)
    {
        var cells = puzzle.Cells.Select(cell => cell.Value.HasValue
                ? Cell.CreateFixed(cell.Row, cell.Column, cell.Value.Value, puzzle.Size)
                : Cell.CreateEmpty(cell.Row, cell.Column, puzzle.Size))
            .ToList();

        return SudokuPuzzle.Create(puzzle.PuzzleId, puzzle.Difficulty, puzzle.Size, cells);
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