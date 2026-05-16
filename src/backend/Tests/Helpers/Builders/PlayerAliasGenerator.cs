using AutoFixture.Kernel;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Helpers.Builders;

public class PlayerAliasGenerator : ISpecimenBuilder
{
    private static readonly string[] Adjectives = ["Swift", "Clever", "Brave", "Witty", "Silent", "Happy"];
    private static readonly string[] Animals = ["Tiger", "Elephant", "Giraffe", "Otter", "Falcon", "Panther"];

    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(PlayerAlias))
        {
            var random = new Random();
            var aliasName = $"{Adjectives[random.Next(Adjectives.Length)]}{Animals[random.Next(Animals.Length)]}{random.Next(10, 100)}";
            return PlayerAlias.Create(aliasName);
        }

        return new NoSpecimen();
    }
}
