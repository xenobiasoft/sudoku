using XenobiaSoft.Sudoku.Extensions;

namespace UnitTests.Sudoku;

public class ExtensionMethodTests
{
    [Fact]
    public void Randomize_ReturnsRandomizedString()
    {
        // Arrange
        List<int> testArray = [1, 2, 3, 4];

        // Act
        var actual = testArray.Randomize();

        // Assert
        actual.Should().BeEquivalentTo(testArray);
    }
}