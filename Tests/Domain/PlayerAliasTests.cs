using DepenMock.XUnit;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Domain;

public class PlayerAliasTests : BaseTestByType<PlayerAlias>
{
    [Theory]
    [InlineData("John")]
    [InlineData("Jane Doe")]
    [InlineData("Player123")]
    [InlineData("AB")]  // Minimum length (2)
    [InlineData("A very long player name with exactly 50 characters")] // Exactly 50 chars
    public void Create_WithValidAlias_CreatesPlayerAlias(string aliasValue)
    {
        // Act
        var alias = PlayerAlias.Create(aliasValue);

        // Assert
        alias.Value.Should().Be(aliasValue.Trim());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithNullOrEmptyAlias_ThrowsInvalidPlayerAliasException(string aliasValue)
    {
        // Act
        Action act = () => PlayerAlias.Create(aliasValue);

        // Assert
        act.Should().Throw<InvalidPlayerAliasException>()
            .WithMessage("Player alias cannot be null or empty");
    }

    [Fact]
    public void Create_WithAliasTooShort_ThrowsInvalidPlayerAliasException()
    {
        // Arrange
        var shortAlias = "A"; // Only 1 character

        // Act
        Action act = () => PlayerAlias.Create(shortAlias);

        // Assert
        act.Should().Throw<InvalidPlayerAliasException>()
            .WithMessage("Player alias must be at least 2 characters");
    }

    [Fact]
    public void Create_WithAliasTooLong_ThrowsInvalidPlayerAliasException()
    {
        // Arrange
        var longAlias = new string('A', 51); // 51 characters

        // Act
        Action act = () => PlayerAlias.Create(longAlias);

        // Assert
        act.Should().Throw<InvalidPlayerAliasException>()
            .WithMessage("Player alias cannot exceed 50 characters");
    }

    [Theory]
    [InlineData("John@Doe")]
    [InlineData("Player#1")]
    [InlineData("Hello!")]
    [InlineData("No-Hyphens")]
    public void Create_WithInvalidCharacters_ThrowsInvalidPlayerAliasException(string aliasValue)
    {
        // Act
        Action act = () => PlayerAlias.Create(aliasValue);

        // Assert
        act.Should().Throw<InvalidPlayerAliasException>()
            .WithMessage("Player alias can only contain letters, numbers, and spaces");
    }

    [Fact]
    public void Create_TrimsSpaces_FromBeginningAndEnd()
    {
        // Arrange
        var aliasWithSpaces = "  John Doe  ";

        // Act
        var alias = PlayerAlias.Create(aliasWithSpaces);

        // Assert
        alias.Value.Should().Be("John Doe");
    }

    [Fact]
    public void ImplicitConversion_ToString_ReturnsValue()
    {
        // Arrange
        var alias = PlayerAlias.Create("John Doe");

        // Act
        string aliasValue = alias;

        // Assert
        aliasValue.Should().Be("John Doe");
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        // Arrange
        var alias = PlayerAlias.Create("John Doe");

        // Act
        var result = alias.ToString();

        // Assert
        result.Should().Be("John Doe");
    }
}