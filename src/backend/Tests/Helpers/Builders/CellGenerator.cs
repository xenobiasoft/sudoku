using AutoFixture.Kernel;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Helpers.Builders;

public class CellGenerator : ISpecimenBuilder
{
    private readonly Random _rnd = Random.Shared;

    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(Cell))
        {
            return Cell.Create(
                _rnd.Next(0, 9),
                _rnd.Next(0, 9),
                _rnd.Next(1, 10)
            );
        }

        return new NoSpecimen();
    }
}