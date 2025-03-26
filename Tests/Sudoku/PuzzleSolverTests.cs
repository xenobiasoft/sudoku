using DepenMock.XUnit;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Solver;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests.Sudoku;

public class PuzzleSolverTests : BaseTestByAbstraction<PuzzleSolver, IPuzzleSolver>
{
	[Fact]
    public async Task TrySolvePuzzle_IfChangesWereMadeToPuzzle_ContinuesLoopingThroughStrategies()
    {
        // Arrange
        var solverStrategy = Container.ResolveMock<SolverStrategy>();
        Container
	        .ResolveMock<IGameStateMemory>()
	        .Setup(x => x.Undo())
	        .Returns(new GameStateMemento(PuzzleFactory.GetSolvedPuzzle(), 10));
        solverStrategy
	        .SetupSequence(x => x.Execute(It.IsAny<Cell[]>()))
	        .Returns(4)
	        .Returns(4)
	        .Throws<InvalidMoveException>();
        var sut = ResolveSut();

        // Act
        await sut.SolvePuzzle(PuzzleFactory.GetPuzzle(Level.ExtremelyHard));

        // Assert
        solverStrategy.Verify(x => x.Execute(It.IsAny<Cell[]>()), Times.Exactly(3));
    }

    [Fact]
	public async Task TrySolvePuzzle_SavesGameState_OnEachLoop()
	{
		// Arrange
		var mockGameState = Container.ResolveMock<IGameStateMemory>();
		var sut = ResolveSut();

		// Act
		await sut.SolvePuzzle(PuzzleFactory.GetSolvedPuzzle());

		// Assert
		mockGameState.Verify(x => x.Save(It.IsAny<GameStateMemento>()), Times.AtLeastOnce);
	}

	[Fact]
	public async Task TrySolvePuzzle_WhenStrategyThrowsInvalidMoveException_GamePopsSavedStateOffStack()
	{
		// Arrange
		Container
			.ResolveMock<IGameStateMemory>()
			.Setup(x => x.Undo())
			.Returns(new GameStateMemento(PuzzleFactory.GetSolvedPuzzle(), 10));
		Container
			.ResolveMock<SolverStrategy>()
			.SetupSequence(x => x.Execute(It.IsAny<Cell[]>()))
			.Throws<InvalidMoveException>()
			.Returns(0);
		var mockGameState = Container.ResolveMock<IGameStateMemory>();
		var sut = ResolveSut();

		// Act
		await sut.SolvePuzzle(PuzzleFactory.GetPuzzle(Level.ExtremelyHard));

		// Assert
		mockGameState.Verify(x => x.Undo(), Times.Once);
	}
}