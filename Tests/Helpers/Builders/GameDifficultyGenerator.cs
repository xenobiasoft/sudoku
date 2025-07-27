using AutoFixture.Kernel;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Helpers.Builders;

public class GameDifficultyGenerator : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(GameDifficulty))
        {
            return GameDifficulty.Easy; // Default to Easy difficulty
        }

        return new NoSpecimen();
    }
}