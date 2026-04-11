using DepenMock.XUnit;
using Sudoku.Application.Common;

namespace UnitTests.Application.Common;

public class ResultTests : BaseTestByType<Result>
{
    [Fact]
    public void Success_CreatesSuccessfulResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().BeNull();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Failure_WithSingleError_CreatesFailureResult()
    {
        // Arrange
        var errorMessage = "Test error";

        // Act
        var result = Result.Failure(errorMessage);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(errorMessage);
        result.Errors.Should().ContainSingle(errorMessage);
    }

    [Fact]
    public void Failure_WithMultipleErrors_CreatesFailureResult()
    {
        // Arrange
        var errors = new List<string> { "Error 1", "Error 2", "Error 3" };

        // Act
        var result = Result.Failure(errors);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(errors.First());
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void Failure_WithEmptyErrorList_CreatesFailureResult()
    {
        // Arrange
        var errors = new List<string>();

        // Act
        var result = Result.Failure(errors);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeNull();
        result.Errors.Should().BeEmpty();
    }
}