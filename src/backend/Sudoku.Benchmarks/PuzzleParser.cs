using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Benchmarks;

/// <summary>
/// Parses an 81-character board string into a <see cref="SudokuPuzzle"/> of fixed clues.
/// Digits 1-9 are clues; '0' or '.' are blanks.
/// </summary>
public static class PuzzleParser
{
    public static SudokuPuzzle Parse(string board, GameDifficulty difficulty)
    {
        if (board.Length != 81)
        {
            throw new ArgumentException($"Board must be 81 characters, got {board.Length}.", nameof(board));
        }

        var cells = new List<Cell>(81);

        for (var i = 0; i < 81; i++)
        {
            int row = i / 9, column = i % 9;
            var ch = board[i];

            cells.Add(ch is >= '1' and <= '9'
                ? Cell.CreateFixed(row, column, ch - '0', BoardSize.Nine)
                : Cell.CreateEmpty(row, column, BoardSize.Nine));
        }

        return SudokuPuzzle.Create(GameId.New(), difficulty, BoardSize.Nine, cells);
    }
}
