using AutoFixture.Kernel;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using UnitTests.Helpers.Factories;

namespace UnitTests.Helpers.Builders;

public class SudokuPuzzleGenerator : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(SudokuPuzzle))
        {
            var cells = CellsFactory.CreateEmptyCells();
            return SudokuPuzzle.Create(
                context.Create<PlayerAlias>(),
                context.Create<GameDifficulty>(),
                cells
            );
        }

        return new NoSpecimen();
    }
}