using Newtonsoft.Json;
using Sudoku.Infrastructure.Models;

namespace UnitTests.Infrastructure.Serialization;

public class GameDifficultyStringConverterTests
{
    [Theory]
    [InlineData("Easy")]
    [InlineData("Medium")]
    [InlineData("Hard")]
    [InlineData("Expert")]
    public void Document_JsonRoundTrip_PreservesDifficulty(string difficulty)
    {
        // Arrange
        var document = new SudokuGameDocument { Difficulty = difficulty };

        // Act
        var json = JsonConvert.SerializeObject(document);
        var restored = JsonConvert.DeserializeObject<SudokuGameDocument>(json);

        // Assert
        restored!.Difficulty.Should().Be(difficulty);
    }

    [Fact]
    public void Document_SerializesDifficultyAsPlainString()
    {
        // Arrange
        var document = new SudokuGameDocument { Difficulty = "Medium" };

        // Act
        var json = JsonConvert.SerializeObject(document);

        // Assert
        json.Should().Contain("\"difficulty\":\"Medium\"");
    }

    [Theory]
    [InlineData("{\"difficulty\":{\"value\":2,\"name\":\"Medium\"}}", "Medium")]
    [InlineData("{\"difficulty\":{\"Value\":3,\"Name\":\"Hard\"}}", "Hard")]
    public void Document_LegacyObjectForm_ReadsDifficultyName(string json, string expected)
    {
        // Act - older documents persisted difficulty as a { value, name } object
        var restored = JsonConvert.DeserializeObject<SudokuGameDocument>(json);

        // Assert
        restored!.Difficulty.Should().Be(expected);
    }
}
