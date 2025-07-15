using DepenMock.XUnit;
using Sudoku.Application.Common;

namespace UnitTests.Application.Common;

public class ResultOfTTests : BaseTestByType<Result<string>>
{
    [Fact]
    public void Success_WithValue_CreatesSuccessfulResult()
    {
        // Arrange
        var value = "Test value";

        // Act
        var result = Result<string>.Success(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
        result.Error.Should().BeNull();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Success_WithNullValue_CreatesSuccessfulResult()
    {
        // Act
        var result = Result<string>.Success(null);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
        result.Error.Should().BeNull();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Failure_WithSingleError_CreatesFailureResult()
    {
        // Arrange
        var errorMessage = "Test error";

        // Act
        var result = Result<string>.Failure(errorMessage);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be(errorMessage);
        result.Errors.Should().ContainSingle(errorMessage);
    }

    [Fact]
    public void Failure_WithMultipleErrors_CreatesFailureResult()
    {
        // Arrange
        var errors = new List<string> { "Error 1", "Error 2", "Error 3" };

        // Act
        var result = Result<string>.Failure(errors);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be(errors.First());
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void Failure_WithEmptyErrorList_CreatesFailureResult()
    {
        // Arrange
        var errors = new List<string>();

        // Act
        var result = Result<string>.Failure(errors);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().BeNull();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Map_WhenSuccessful_TransformsValue()
    {
        // Arrange
        var originalValue = "test";
        var result = Result<string>.Success(originalValue);

        // Act
        var mappedResult = result.Map(s => s.ToUpper());

        // Assert
        mappedResult.IsSuccess.Should().BeTrue();
        mappedResult.Value.Should().Be("TEST");
        mappedResult.Error.Should().BeNull();
        mappedResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Map_WhenFailure_PreservesError()
    {
        // Arrange
        var errorMessage = "Test error";
        var result = Result<string>.Failure(errorMessage);

        // Act
        var mappedResult = result.Map(s => s.ToUpper());

        // Assert
        mappedResult.IsSuccess.Should().BeFalse();
        mappedResult.Value.Should().BeNull();
        mappedResult.Error.Should().Be(errorMessage);
        mappedResult.Errors.Should().ContainSingle(errorMessage);
    }

    [Fact]
    public void Map_WhenFailureWithMultipleErrors_PreservesAllErrors()
    {
        // Arrange
        var errors = new List<string> { "Error 1", "Error 2" };
        var result = Result<string>.Failure(errors);

        // Act
        var mappedResult = result.Map(s => s.ToUpper());

        // Assert
        mappedResult.IsSuccess.Should().BeFalse();
        mappedResult.Value.Should().BeNull();
        mappedResult.Error.Should().Be(errors.First());
        mappedResult.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void Map_WithComplexTransformation_WorksCorrectly()
    {
        // Arrange
        var originalValue = "123";
        var result = Result<string>.Success(originalValue);

        // Act
        var mappedResult = result.Map(s => int.Parse(s));

        // Assert
        mappedResult.IsSuccess.Should().BeTrue();
        mappedResult.Value.Should().Be(123);
        mappedResult.Error.Should().BeNull();
        mappedResult.Errors.Should().BeEmpty();
    }
}