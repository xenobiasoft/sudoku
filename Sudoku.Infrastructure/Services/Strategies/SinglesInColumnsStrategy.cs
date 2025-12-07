using Sudoku.Domain.Entities;

namespace Sudoku.Infrastructure.Services.Strategies;

public class SinglesInColumnsStrategy : SolverStrategy
{
	public override bool Execute(SudokuPuzzle puzzle)
	{
		var colPos = 0;
		var rowPos = 0;
        var changesMade = false;

		for (var col = 0; col < 9; col++)
		{
			for (var number = 1; number <= 9; number++)
			{
				var occurrence = 0;

				foreach (var colCell in puzzle.GetColumnCells(col))
				{
					if (colCell.Value.HasValue || !colCell.PossibleValues.Contains(number)) continue;

					occurrence += 1;

					if (occurrence > 1)
					{
						break;
					}

					colPos = colCell.Column;
					rowPos = colCell.Row;
				}

				if (occurrence != 1) continue;

				var loneRangerCell = puzzle.GetCell(rowPos, colPos);
				Console.WriteLine($"Setting cell:{loneRangerCell.Row}:{loneRangerCell.Column} to value {number}");
				loneRangerCell.SetValue(number);
				loneRangerCell.PossibleValues.Clear();
				changesMade = true;
            }
		}

		return changesMade;
    }

    public override int Order { get; } = 1;
}