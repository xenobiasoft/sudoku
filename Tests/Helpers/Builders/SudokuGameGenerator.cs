using AutoFixture.Kernel;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Helpers.Builders;

public class SudokuGameGenerator : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(SudokuGame))
        {
            var cells = new List<Cell>();

            for (var row = 0; row < 9; row++)
            {
                for (var col = 0; col < 9; col++)
                {
                    cells.Add(Cell.Create(row, col));
                }
            }

            return SudokuGame.Create(
                context.Create<PlayerAlias>(),
                context.Create<GameDifficulty>(),
                cells
            );
        }

        return new NoSpecimen();
    }
}