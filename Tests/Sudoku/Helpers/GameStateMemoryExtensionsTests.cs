using DepenMock.XUnit;
using XenobiaSoft.Sudoku.Extensions;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Sudoku.Helpers;

public class GameStateMemoryExtensionsTests : BaseTest
{
	[Fact]
	public void IsSameGameStateAs_WhenGivenSameBoard_ReturnsTrue()
	{
		// Arrange
        var sut = Container.Create<GameStateMemory>();
        var clonedBoard = sut.Clone();

        // Act
        var isSame = sut.IsSameGameStateAs(clonedBoard);

        // Assert
		isSame.Should().BeTrue();
    }

	[Fact]
	public void IsSameGameStateAs_WhenBoardCellValueHasChanged_ReturnsFalse()
	{
		// Arrange
		var sut = Container.Create<GameStateMemory>();
        var clonedBoard = sut.Clone();
		clonedBoard.Board[0].Value = 5;

        // Act
        var isSame = sut.IsSameGameStateAs(clonedBoard);

        // Assert
        isSame.Should().BeFalse();
    }

    [Fact]
    public void IsSameGameStateAs_WhenBoardCellPossibleMovesHasChanged_ReturnsFalse()
    {
        // Arrange
        var sut = Container.Create<GameStateMemory>();
        var clonedBoard = sut.Clone();
        clonedBoard.Board[0].PossibleValues = new List<int>([1, 2, 3, 4, 5]);

        // Act
        var isSame = sut.IsSameGameStateAs(clonedBoard);

        // Assert
        isSame.Should().BeFalse();
    }

    [Fact]
	public void IsSameGameStateAs_WhenTotalMovesMadeChanges_ReturnsTrue()
	{
        // Arrange
        var sut = Container.Create<GameStateMemory>();
        var clonedBoard = sut.Clone();
        clonedBoard.TotalMoves = sut.TotalMoves + 1;

        // Act
        var isSame = sut.IsSameGameStateAs(clonedBoard);

        // Assert
        isSame.Should().BeTrue();
    }

    [Fact]
    public void IsSameGameStateAs_WhenInvalidMovesMadeChanges_ReturnsTrue()
    {
        // Arrange
        var sut = Container.Create<GameStateMemory>();
        var clonedBoard = sut.Clone();
        clonedBoard.InvalidMoves = sut.InvalidMoves + 1;

        // Act
        var isSame = sut.IsSameGameStateAs(clonedBoard);

        // Assert
        isSame.Should().BeTrue();
    }
}