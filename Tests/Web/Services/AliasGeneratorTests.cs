using Sudoku.Web.Server.Services;

namespace UnitTests.Web.Services;

public class AliasGeneratorTests
{
    [Fact]
    public void GenerateAlias_ShouldReturnNonNullOrEmptyString()
    {
        // Act
        var alias = AliasGenerator.GenerateAlias();

        // Assert
        alias.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateAlias_ShouldContainAdjectiveAndAnimalAndNumber()
    {
        // Arrange
        var adjectives = new[] { "Swift", "Clever", "Brave", "Witty", "Silent", "Happy" };
        var animals = new[] { "Tiger", "Elephant", "Giraffe", "Otter", "Falcon", "Panther" };

        // Act
        var alias = AliasGenerator.GenerateAlias();

        // Assert
        var adjective = adjectives.FirstOrDefault(a => alias.StartsWith(a));
        adjective.Should().NotBeNull();

        var animal = animals.FirstOrDefault(a => alias.Contains(a));
        animal.Should().NotBeNull();

        var numberPart = alias.Replace(adjective!, "").Replace(animal!, "");
        int.TryParse(numberPart, out var number).Should().BeTrue();
        number.Should().BeGreaterThanOrEqualTo(10).And.BeLessThan(100);
    }

    [Fact]
    public void GenerateAlias_ShouldBeRandomized()
    {
        // Act
        var aliases = Enumerable.Range(0, 20)
            .Select(_ => AliasGenerator.GenerateAlias())
            .ToList();

        // Assert
        aliases.Distinct().Count().Should().BeGreaterThan(1);
    }
}