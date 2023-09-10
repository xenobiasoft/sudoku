namespace UnitTests;

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
        actual.Should().NotBe(testString).And.Subject.Length.Should().Be(testString.Length);
    }
}