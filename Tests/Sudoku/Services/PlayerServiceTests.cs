using DepenMock.XUnit;
using XenobiaSoft.Sudoku.Abstractions;
using XenobiaSoft.Sudoku.Services;

namespace UnitTests.Sudoku.Services;

public class PlayerServiceTests : BaseTestByAbstraction<PlayerService, IPlayerService>
{
    private readonly IPlayerService _sut;

    public PlayerServiceTests()
    {
        _sut = ResolveSut();
    }

    [Fact]
    public async Task GenerateAlias_ShouldReturnNonNullOrEmptyString()
    {
        // Act
        var alias = await _sut.CreateNewAsync();

        // Assert
        alias.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GenerateAlias_ShouldContainAdjectiveAndAnimalAndNumber()
    {
        // Arrange
        var adjectives = new[] { "Swift", "Clever", "Brave", "Witty", "Silent", "Happy" };
        var animals = new[] { "Tiger", "Elephant", "Giraffe", "Otter", "Falcon", "Panther" };

        // Act
        var alias = await _sut.CreateNewAsync();

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
    public async Task GenerateAlias_ShouldBeRandomized()
    {
        // Act
        var selectTasks = Enumerable.Range(0, 20).Select(_ => _sut.CreateNewAsync());
        var aliases = (await Task.WhenAll(selectTasks)).ToList();

        // Assert
        aliases.Distinct().Count().Should().Be(aliases.Count);
    }
}