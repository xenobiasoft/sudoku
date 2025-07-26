using AutoFixture.Kernel;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Helpers.Builders;

public class SudokuGameClassBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(SudokuGame))
        {
            return SudokuGame.Create(
                context.Create<PlayerAlias>(),
                context.Create<GameDifficulty>(),
                context.CreateMany<Cell>(81)
            );
        }

        return new NoSpecimen();
    }
}