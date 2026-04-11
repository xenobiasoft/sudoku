using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Infrastructure.Services.Strategies;

public abstract class TripletsStrategyBase : SolverStrategy
{
    protected bool HandleNakedTriplets(List<Cell> cells)
    {
        var changesMade = false;

        // Check all combinations of 3 cells
        foreach (var combo in GetTripletCombinations(cells))
        {
            if (combo.Count(x => !x.HasValue) != 3)
            {
                continue;
            }

            var combined = combo.SelectMany(c => c.PossibleValues).Distinct().ToList();

            // Must have exactly 3 unique values
            if (combined.Count != 3) continue;

            var tripletValues = new HashSet<int>(combined);

            // Validate it's a proper naked triplet:
            // 1. Each triplet value appears in at least one cell
            if (!tripletValues.All(num => combo.Any(c => c.PossibleValues.Contains(num)))) continue;

            // 2. Each cell contains only values from the triplet
            if (!combo.All(c => c.PossibleValues.IsSubsetOf(tripletValues))) continue;

            // Valid naked triplet found - eliminate from other cells
            foreach (var otherCell in cells.Except(combo))
            {
                if (otherCell.HasValue) continue;

                var before = otherCell.PossibleValues.Count;
                otherCell.PossibleValues.RemoveWhere(x => tripletValues.Contains(x));

                if (!otherCell.PossibleValues.Any())
                {
                    throw new InvalidMoveException($"Invalid move at row:{otherCell.Row}, col:{otherCell.Column}");
                }

                if (otherCell.PossibleValues.Count == 1 && before > 1)
                {
                    var val = otherCell.PossibleValues.First();
                    otherCell.SetValue(val);
                    otherCell.PossibleValues.Clear();
                    changesMade = true;
                }
                else if (otherCell.PossibleValues.Count < before)
                {
                    changesMade = true;
                }
            }
        }

        return changesMade;
    }

    protected bool HandleHiddenTriplets(List<Cell> cells)
    {
        var changesMade = false;
        var numbers = Enumerable.Range(1, 9).ToList();

        // Check all combinations of 3 numbers
        for (var i = 0; i < numbers.Count - 2; i++)
        {
            for (var j = i + 1; j < numbers.Count - 1; j++)
            {
                for (var k = j + 1; k < numbers.Count; k++)
                {
                    var tripletNumbers = new HashSet<int> { numbers[i], numbers[j], numbers[k] };

                    // Find cells that contain ANY of these numbers
                    var cellsWithAnyNumber = cells
                        .Where(c => c.PossibleValues.Overlaps(tripletNumbers))
                        .ToList();

                    // Hidden triplet: these 3 numbers appear in exactly 3 cells
                    if (cellsWithAnyNumber.Count != 3) continue;

                    // Verify each number in triplet appears in at least one of these cells
                    if (!tripletNumbers.All(num => cellsWithAnyNumber.Any(c => c.PossibleValues.Contains(num)))) continue;

                    // Valid hidden triplet - remove other candidates from these cells
                    foreach (var cell in cellsWithAnyNumber)
                    {
                        var before = cell.PossibleValues.Count;
                        cell.PossibleValues.RemoveWhere(v => !tripletNumbers.Contains(v));

                        if (!cell.PossibleValues.Any())
                        {
                            throw new InvalidMoveException($"Invalid move at {cell.Row},{cell.Column}");
                        }

                        if (cell.PossibleValues.Count == 1 && before > 1)
                        {
                            var val = cell.PossibleValues.First();
                            cell.SetValue(val);
                            cell.PossibleValues.Clear();
                            changesMade = true;
                        }
                        else if (cell.PossibleValues.Count < before)
                        {
                            changesMade = true;
                        }
                    }
                }
            }
        }

        return changesMade;
    }

    private static IEnumerable<List<Cell>> GetTripletCombinations(List<Cell> cells)
    {
        for (var i = 0; i < cells.Count - 2; i++)
        {
            for (var j = i + 1; j < cells.Count - 1; j++)
            {
                for (var l = j + 1; l < cells.Count; l++)
                {
                    yield return [cells[i], cells[j], cells[l]];
                }
            }
        }
    }
}