using AutoFixture.Kernel;
using Sudoku.Domain.ValueObjects;
using Sudoku.Web.Server.Services;

namespace UnitTests.Helpers.Builders;

public class PlayerAliasGenerator : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(PlayerAlias))
        {
            var aliasName = AliasGenerator.GenerateAlias();

            return PlayerAlias.Create(aliasName);
        }

        return new NoSpecimen();
    }
}