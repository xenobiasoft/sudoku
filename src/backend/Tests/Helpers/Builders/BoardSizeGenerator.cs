using AutoFixture.Kernel;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Helpers.Builders;

public class BoardSizeGenerator : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(BoardSize))
        {
            return BoardSize.Nine; // Default to the classic 9x9 board
        }

        return new NoSpecimen();
    }
}
