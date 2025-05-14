using XenobiaSoft.Sudoku.Extensions;

namespace UnitTests.Sudoku;

public class StringExtensionMethodTests
{
    [Fact]
    public void Randomize_ReturnsRandomizedString()
    {
        // Arrange
        var testString = "abcd";

        // Act
        var actual = testString.Randomize();

        // Assert
        actual.Length.Should().Be(testString.Length);
    }
}