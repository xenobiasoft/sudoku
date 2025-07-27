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
            var cells = CellsFactory.CreateEmptyCells();
            return SudokuGame.Create(
                context.Create<PlayerAlias>(),
                context.Create<GameDifficulty>(),
                cells
            );
        }

        return new NoSpecimen();
    }
}