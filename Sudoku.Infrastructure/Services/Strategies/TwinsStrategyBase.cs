using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Infrastructure.Services.Strategies;

public abstract class TwinsStrategyBase : SolverStrategy
{
    protected bool HandleNakedTwins(IEnumerable<IEnumerable<Cell>> groups)
    {
        var changesMade = false;

        foreach (var group in groups)
        {
            var cells = group.Where(c => !c.Value.HasValue && c.PossibleValues.Count == 2).ToList();

            for (var i = 0; i < cells.Count; i++)
            {
                for (var j = i + 1; j < cells.Count; j++)
                {
                    if (!cells[i].PossibleValues.SetEquals(cells[j].PossibleValues)) continue;

                    var twinValues = cells[i].PossibleValues;

                    foreach (var nonTwinCell in group)
                    {
                        if (nonTwinCell == cells[i] || nonTwinCell == cells[j] || nonTwinCell.Value.HasValue) continue;

                        var removedCount = nonTwinCell.PossibleValues.RemoveWhere(x => twinValues.Contains(x));
                        if (removedCount <= 0) continue;

                        changesMade = true;

                        if (!nonTwinCell.PossibleValues.Any())
                        {
                            throw new InvalidMoveException($"Invalid move for position: {nonTwinCell.Row}, {nonTwinCell.Column}");
                        }

                        if (nonTwinCell.PossibleValues.Count != 1) continue;

                        var cellValue = nonTwinCell.PossibleValues.First();
                        nonTwinCell.SetValue(cellValue);
                        nonTwinCell.PossibleValues.Clear();
                    }
                }
            }
        }

        return changesMade;
    }
}