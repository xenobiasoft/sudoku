using DepenMock.XUnit;
using Sudoku.Domain.Exceptions;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Domain;

public class CellTests : BaseTestByType<Cell>
{
    [Fact]
    public void Create_WithValidParameters_CreatesCell()
    {
        // Arrange
        int row = 4;
        int column = 5;
        int? value = 7;
        bool isFixed = true;

        // Act
        var cell = Cell.Create(row, column, value, isFixed);

        // Assert
        cell.Row.Should().Be(row);
        cell.Column.Should().Be(column);
        cell.Value.Should().Be(value);
        cell.IsFixed.Should().Be(isFixed);
        cell.HasValue.Should().BeTrue();
    }

    [Fact]
    public void CreateFixed_WithValidParameters_CreatesFixedCell()
    {
        // Arrange
        int row = 4;
        int column = 5;
        int value = 7;

        // Act
        var cell = Cell.CreateFixed(row, column, value);

        // Assert
        cell.Row.Should().Be(row);
        cell.Column.Should().Be(column);
        cell.Value.Should().Be(value);
        cell.IsFixed.Should().BeTrue();
        cell.HasValue.Should().BeTrue();
    }

    [Fact]
    public void CreateEmpty_WithValidParameters_CreatesEmptyCell()
    {
        // Arrange
        int row = 4;
        int column = 5;

        // Act
        var cell = Cell.CreateEmpty(row, column);

        // Assert
        cell.Row.Should().Be(row);
        cell.Column.Should().Be(column);
        cell.Value.Should().BeNull();
        cell.IsFixed.Should().BeFalse();
        cell.HasValue.Should().BeFalse();
    }

    [Theory]
    [InlineData(-1, 5)]
    [InlineData(9, 5)]
    [InlineData(10, 5)]
    public void Create_WithInvalidRow_ThrowsInvalidCellPositionException(int row, int column)
    {
        // Act
        Action act = () => Cell.Create(row, column);

        // Assert
        act.Should().Throw<InvalidCellPositionException>()
            .WithMessage($"*Row must be between 0 and 8*");
    }

    [Theory]
    [InlineData(5, -1)]
    [InlineData(5, 9)]
    [InlineData(5, 10)]
    public void Create_WithInvalidColumn_ThrowsInvalidCellPositionException(int row, int column)
    {
        // Act
        Action act = () => Cell.Create(row, column);

        // Assert
        act.Should().Throw<InvalidCellPositionException>()
            .WithMessage($"*Column must be between 0 and 8*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(-1)]
    public void Create_WithInvalidValue_ThrowsInvalidCellValueException(int value)
    {
        // Act
        Action act = () => Cell.Create(5, 5, value);

        // Assert
        act.Should().Throw<InvalidCellValueException>()
            .WithMessage($"*Cell value must be between 1 and 9*");
    }

    [Fact]
    public void SetValue_OnNonFixedCell_UpdatesValue()
    {
        // Arrange
        var cell = Cell.Create(5, 5);

        // Act
        cell.SetValue(7);

        // Assert
        cell.Value.Should().Be(7);
        cell.HasValue.Should().BeTrue();
    }

    [Fact]
    public void SetValue_OnFixedCell_ThrowsCellIsFixedException()
    {
        // Arrange
        var cell = Cell.CreateFixed(5, 5, 7);

        // Act
        Action act = () => cell.SetValue(8);

        // Assert
        act.Should().Throw<CellIsFixedException>()
            .WithMessage($"*Cannot modify fixed cell at position (5, 5)*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(-1)]
    public void SetValue_WithInvalidValue_ThrowsInvalidCellValueException(int value)
    {
        // Arrange
        var cell = Cell.Create(5, 5);

        // Act
        Action act = () => cell.SetValue(value);

        // Assert
        act.Should().Throw<InvalidCellValueException>()
            .WithMessage($"*Cell value must be between 1 and 9*");
    }

    [Fact]
    public void SetValue_WithNullableInt_OnNonFixedCell_UpdatesValue()
    {
        // Arrange
        var cell = Cell.Create(5, 5);

        // Act
        cell.SetValue((int?)7);

        // Assert
        cell.Value.Should().Be(7);
        cell.HasValue.Should().BeTrue();
    }

    [Fact]
    public void SetValue_WithNullableInt_OnFixedCell_ThrowsCellIsFixedException()
    {
        // Arrange
        var cell = Cell.CreateFixed(5, 5, 7);

        // Act
        Action act = () => cell.SetValue((int?)8);

        // Assert
        act.Should().Throw<CellIsFixedException>()
            .WithMessage($"*Cannot modify fixed cell at position (5, 5)*");
    }

    [Fact]
    public void SetValue_WithNullValue_ClearsValue()
    {
        // Arrange
        var cell = Cell.Create(5, 5, 7);

        // Act
        cell.SetValue((int?)null);

        // Assert
        cell.Value.Should().BeNull();
        cell.HasValue.Should().BeFalse();
    }

    [Fact]
    public void SetValue_WithNullValue_OnFixedCell_ThrowsCellIsFixedException()
    {
        // Arrange
        var cell = Cell.CreateFixed(5, 5, 7);

        // Act
        Action act = () => cell.SetValue((int?)null);

        // Assert
        act.Should().Throw<CellIsFixedException>()
            .WithMessage($"*Cannot modify fixed cell at position (5, 5)*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(-1)]
    public void SetValue_WithNullableInt_WithInvalidValue_ThrowsInvalidCellValueException(int value)
    {
        // Arrange
        var cell = Cell.Create(5, 5);

        // Act
        Action act = () => cell.SetValue((int?)value);

        // Assert
        act.Should().Throw<InvalidCellValueException>()
            .WithMessage($"*Cell value must be between 1 and 9*");
    }

    [Fact]
    public void ClearValue_OnNonFixedCell_ClearsValue()
    {
        // Arrange
        var cell = Cell.Create(5, 5, 7);

        // Act
        cell.ClearValue();

        // Assert
        cell.Value.Should().BeNull();
        cell.HasValue.Should().BeFalse();
    }

    [Fact]
    public void ClearValue_OnFixedCell_ThrowsCellIsFixedException()
    {
        // Arrange
        var cell = Cell.CreateFixed(5, 5, 7);

        // Act
        Action act = () => cell.ClearValue();

        // Assert
        act.Should().Throw<CellIsFixedException>()
            .WithMessage($"*Cannot modify fixed cell at position (5, 5)*");
    }

    [Fact]
    public void ToString_WithValue_ReturnsValueAsString()
    {
        // Arrange
        var cell = Cell.Create(5, 5, 7);

        // Act
        var result = cell.ToString();

        // Assert
        result.Should().Be("7");
    }

    [Fact]
    public void ToString_WithoutValue_ReturnsDot()
    {
        // Arrange
        var cell = Cell.CreateEmpty(5, 5);

        // Act
        var result = cell.ToString();

        // Assert
        result.Should().Be(".");
    }
}