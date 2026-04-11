using DepenMock.XUnit;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Domain;

public class GameDifficultyTests : BaseTestByType<GameDifficulty>
{
    [Fact]
    public void PredefinedDifficulties_HaveCorrectValues()
    {
        // Assert
        GameDifficulty.Easy.Value.Should().Be(1);
        GameDifficulty.Easy.Name.Should().Be("Easy");

        GameDifficulty.Medium.Value.Should().Be(2);
        GameDifficulty.Medium.Name.Should().Be("Medium");

        GameDifficulty.Hard.Value.Should().Be(3);
        GameDifficulty.Hard.Name.Should().Be("Hard");

        GameDifficulty.Expert.Value.Should().Be(4);
        GameDifficulty.Expert.Name.Should().Be("Expert");
    }

    [Theory]
    [InlineData(1, "Easy")]
    [InlineData(2, "Medium")]
    [InlineData(3, "Hard")]
    [InlineData(4, "Expert")]
    public void FromValue_WithValidValue_ReturnsCorrectDifficulty(int value, string expectedName)
    {
        // Act
        var difficulty = GameDifficulty.FromValue(value);

        // Assert
        difficulty.Value.Should().Be(value);
        difficulty.Name.Should().Be(expectedName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(-1)]
    public void FromValue_WithInvalidValue_ThrowsInvalidGameDifficultyException(int invalidValue)
    {
        // Act
        Action act = () => GameDifficulty.FromValue(invalidValue);

        // Assert
        act.Should().Throw<InvalidGameDifficultyException>()
            .WithMessage($"Invalid difficulty value: {invalidValue}");
    }

    [Theory]
    [InlineData("easy", "Easy")]
    [InlineData("EASY", "Easy")]
    [InlineData("medium", "Medium")]
    [InlineData("MEDIUM", "Medium")]
    [InlineData("hard", "Hard")]
    [InlineData("HARD", "Hard")]
    [InlineData("expert", "Expert")]
    [InlineData("EXPERT", "Expert")]
    public void FromName_WithValidName_ReturnsCorrectDifficulty(string name, string expectedName)
    {
        // Act
        var difficulty = GameDifficulty.FromName(name);

        // Assert
        difficulty.Name.Should().Be(expectedName);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("")]
    [InlineData(null)]
    public void FromName_WithInvalidName_ThrowsInvalidGameDifficultyException(string invalidName)
    {
        // Act
        Action act = () => GameDifficulty.FromName(invalidName);

        // Assert
        act.Should().Throw<InvalidGameDifficultyException>()
            .WithMessage($"Invalid difficulty name: {invalidName}");
    }

    [Fact]
    public void ImplicitConversion_ToInt_ReturnsValue()
    {
        // Arrange
        var difficulty = GameDifficulty.Medium;

        // Act
        int value = difficulty;

        // Assert
        value.Should().Be(2);
    }

    [Fact]
    public void ImplicitConversion_ToString_ReturnsName()
    {
        // Arrange
        var difficulty = GameDifficulty.Medium;

        // Act
        string name = difficulty;

        // Assert
        name.Should().Be("Medium");
    }

    [Fact]
    public void ToString_ReturnsName()
    {
        // Arrange
        var difficulty = GameDifficulty.Medium;

        // Act
        var result = difficulty.ToString();

        // Assert
        result.Should().Be("Medium");
    }

    [Fact]
    public void Equality_WithSameValue_AreEqual()
    {
        // Arrange
        var difficulty1 = GameDifficulty.Medium;
        var difficulty2 = GameDifficulty.FromValue(2);

        // Act & Assert
        difficulty1.Should().Be(difficulty2);
    }

    [Fact]
    public void Equality_WithDifferentValues_AreNotEqual()
    {
        // Arrange
        var difficulty1 = GameDifficulty.Easy;
        var difficulty2 = GameDifficulty.Medium;

        // Act & Assert
        difficulty1.Should().NotBe(difficulty2);
    }
}