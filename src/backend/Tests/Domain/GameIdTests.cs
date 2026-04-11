using DepenMock.XUnit;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Domain;

public class GameIdTests : BaseTestByType<GameId>
{
    [Fact]
    public void New_CreatesGameIdWithNewGuid()
    {
        // Act
        var gameId = GameId.New();

        // Assert
        gameId.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_WithGuid_CreatesGameIdWithCorrectValue()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var gameId = GameId.Create(guid);

        // Assert
        gameId.Value.Should().Be(guid);
    }

    [Fact]
    public void Create_WithValidString_CreatesGameIdWithCorrectValue()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var guidString = guid.ToString();

        // Act
        var gameId = GameId.Create(guidString);

        // Assert
        gameId.Value.Should().Be(guid);
    }

    [Theory]
    [InlineData("not-a-guid")]
    [InlineData("")]
    public void Create_WithInvalidString_ThrowsFormatException(string invalidGuid)
    {
        // Act
        Action act = () => GameId.Create(invalidGuid);

        // Assert
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ImplicitConversion_ToGuid_ReturnsValue()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var gameId = GameId.Create(guid);

        // Act
        Guid result = gameId;

        // Assert
        result.Should().Be(guid);
    }

    [Fact]
    public void ImplicitConversion_ToString_ReturnsValueAsString()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var gameId = GameId.Create(guid);

        // Act
        string result = gameId;

        // Assert
        result.Should().Be(guid.ToString());
    }

    [Fact]
    public void ToString_ReturnsValueAsString()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var gameId = GameId.Create(guid);

        // Act
        var result = gameId.ToString();

        // Assert
        result.Should().Be(guid.ToString());
    }

    [Fact]
    public void Equality_WithSameValue_AreEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var gameId1 = GameId.Create(guid);
        var gameId2 = GameId.Create(guid);

        // Act & Assert
        gameId1.Should().Be(gameId2);
    }

    [Fact]
    public void Equality_WithDifferentValues_AreNotEqual()
    {
        // Arrange
        var gameId1 = GameId.Create(Guid.NewGuid());
        var gameId2 = GameId.Create(Guid.NewGuid());

        // Act & Assert
        gameId1.Should().NotBe(gameId2);
    }
}