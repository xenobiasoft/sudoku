using DepenMock.XUnit;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.PuzzleSolver;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests;

public class PuzzleSolverTests : BaseTestByAbstraction<PuzzleSolver, IPuzzleSolver>
{
    [Fact]
    public void TrySolvePuzzle_IfChangesWereMadeToPuzzle_ContinuesLoopingThroughStrategies()
    {
        // Arrange
        var solverStrategy = Container.ResolveMock<SolverStrategy>();
        solverStrategy
            .SetupSequence(x => x.Execute(It.IsAny<Cell[]>()))
            .Returns(4)
            .Returns(4)
            .Returns(0);
        var sut = ResolveSut();

        // Act
        sut.TrySolvePuzzle(PuzzleFactory.GetPuzzle(Level.ExtremelyHard));

        // Assert
        solverStrategy.Verify(x => x.Execute(It.IsAny<Cell[]>()), Times.Exactly(3));
    }

    [Fact]
    public void IsSolved_WhenPuzzleIsValidAndAllValuesPopulatedWithNumber_ReturnsTrue()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetSolvedPuzzle();
        var sut = ResolveSut();

        // Act
        var isSolved = sut.IsSolved(puzzle);

        // Assert
        isSolved.Should().BeTrue();
	}
}