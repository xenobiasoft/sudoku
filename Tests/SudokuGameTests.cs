using DepenMock.XUnit;
using UnitTests.CustomAssertions;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Helpers;
using XenobiaSoft.Sudoku.PuzzleSolver;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests;

public class SudokuGameTests : BaseTestByAbstraction<SudokuGame, ISudokuGame>
{
	[Fact]
	public void SaveGameState_StoresCurrentGameStateOnStack()
	{
		// Arrange
		var mockGameMemory = Container.ResolveMock<IGameStateMemory>();
		var sut = ResolveSut();

		// Act
		sut.SaveGameState();

		// Assert
		mockGameMemory.Verify(x => x.Save(It.IsAny<GameStateMemento>()), Times.Once);
	}

	[Fact]
	public void Undo_RestoresScoreFromSavedGameState()
	{
		// Arrange
		var gameState = new GameStateMemento(Container.CreateMany<Cell>(), Container.Create<int>());
		Container
			.ResolveMock<IGameStateMemory>()
			.Setup(x => x.Undo())
			.Returns(gameState);
		var sut = ResolveSut();

		// Act
		sut.Undo();

		// Assert
		Assert.Equal(gameState.Score, sut.Score);
	}

	[Fact]
	public void Undo_RestoresPuzzleFromSavedGameState()
	{
		// Arrange
		var cells = Container.CreateMany<Cell>();
		var gameState = new GameStateMemento(cells, Container.Create<int>());
		Container
			.ResolveMock<IGameStateMemory>()
			.Setup(x => x.Undo())
			.Returns(gameState);
		var sut = ResolveSut();

		// Act
		sut.Undo();

		// Assert
		sut.Puzzle.Should().BeEquivalentTo(cells.ToArray());
	}

	[Fact]
	public void SolvePuzzle_CallsPuzzleSolver()
	{
		// Arrange
		var mockPuzzleSolver = Container.ResolveMock<IPuzzleSolver>();
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle();

		// Assert
		mockPuzzleSolver.Verify(x => x.TrySolvePuzzle(It.IsAny<Cell[]>()), Times.Once);
	}

	[Fact]
	public void SolvePuzzle_SetsScoreBasedOnPuzzleSolverScore()
	{
		// Arrange
		const int expectedScore = 45;
		Container
			.ResolveMock<IPuzzleSolver>()
			.Setup(x => x.TrySolvePuzzle(It.IsAny<Cell[]>()))
			.Returns(expectedScore);
		var sut = ResolveSut();
		var currentScore = sut.Score;

		// Act
		sut.SolvePuzzle();

		// Assert
		sut.Score.Should().Be(currentScore + expectedScore);
	}

	[Fact]
	public void SolvePuzzle_WhenUsingBruteForceMethod_SavesGameStateToMemento()
	{
		// Arrange
		Container
			.ResolveMock<IPuzzleSolver>()
			.SetupSequence(x => x.IsSolved(It.IsAny<Cell[]>()))
			.Returns(false)
			.Returns(true);
		Container
			.ResolveMock<IPuzzleSolver>()
			.Setup(x => x.TrySolvePuzzle(It.IsAny<Cell[]>()))
			.Returns(0);
		var mockGameState = Container.ResolveMock<IGameStateMemory>();
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle();

		// Assert
		mockGameState.Verify(x => x.Save(It.IsAny<GameStateMemento>()), Times.Once);
	}

	[Fact]
	public void SolvePuzzle_WhenStrategyThrowsInvalidOperationException_GamePopsSavedStateOffStack()
	{
		// Arrange
		var mockGameState = Container.ResolveMock<IGameStateMemory>();
		Container
			.ResolveMock<IPuzzleSolver>()
			.Setup(x => x.TrySolvePuzzle(It.IsAny<Cell[]>()))
			.Throws<InvalidOperationException>();
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle();

		// Assert
		mockGameState.Verify(x => x.Undo(), Times.AtLeastOnce);
	}

	[Fact]
	public void SolvePuzzle_WhenUsingBruteForceMethod_SetsCellWithFewestPossibleValues()
	{
		// Arrange
		Container
			.ResolveMock<IPuzzleSolver>()
			.SetupSequence(x => x.IsSolved(It.IsAny<Cell[]>()))
			.Returns(false)
			.Returns(true);
		Container
			.ResolveMock<IPuzzleSolver>()
			.Setup(x => x.TrySolvePuzzle(It.IsAny<Cell[]>()))
			.Returns(0);
		var sut = ResolveSut();
		sut.Restore(PuzzleFactory.GetPuzzle(Level.ExtremelyHard));
		sut.Puzzle.PopulatePossibleValues();

		// Act
		sut.SolvePuzzle();

		// Assert
		sut.Puzzle.GetCell(3, 5).Value.Should().BeOneOf(5, 9);
	}

	[Fact]
	public void SolvePuzzle_WhenUsingBruteForceMethod_AddsFivePointsToExistingScore()
	{
		// Arrange
		Container
			.ResolveMock<IPuzzleSolver>()
			.SetupSequence(x => x.IsSolved(It.IsAny<Cell[]>()))
			.Returns(false)
			.Returns(false)
			.Returns(true);
		var puzzleSolverScore = 45;
		Container
			.ResolveMock<IPuzzleSolver>()
			.SetupSequence(x => x.TrySolvePuzzle(It.IsAny<Cell[]>()))
			.Returns(puzzleSolverScore)
			.Returns(0);
		var sut = ResolveSut();
		sut.Restore(PuzzleFactory.GetPuzzle(Level.ExtremelyHard));
		sut.Puzzle.PopulatePossibleValues();
		var gameScore = sut.Score;

		// Act
		sut.SolvePuzzle();

		// Assert
		sut.Score.Should().Be(gameScore + puzzleSolverScore + 5);
	}

	[Fact]
	public void SolvePuzzle_WhenUsingBruteForceMethod_MakesMaxFiftyAttempts()
	{
		// Arrange
		var mockPuzzleSolver = Container.ResolveMock<IPuzzleSolver>();
		mockPuzzleSolver
			.Setup(x => x.IsSolved(It.IsAny<Cell[]>()))
			.Returns(false);
		mockPuzzleSolver
			.Setup(x => x.TrySolvePuzzle(It.IsAny<Cell[]>()))
			.Returns(0);
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle();

		// Assert
		mockPuzzleSolver.Verify(x => x.TrySolvePuzzle(It.IsAny<Cell[]>()), Times.Exactly(50));
	}

	[Fact]
	public void SetCell_AcceptsValue_AndSetsCell()
	{
		// Arrange
		var sut = ResolveSut();
		sut.Restore(PuzzleFactory.GetSolvedPuzzle());

		// Act
		sut.SetCell(2, 1, 5);

		// Assert
		sut.Puzzle.GetCell(2, 1).Value.Should().Be(5);
	}

	[Theory]
	[InlineData(5, 9, 6, "Invalid column (Parameter 'col')")]
	[InlineData(9, 5, 6, "Invalid row (Parameter 'row')")]
	[InlineData(1, 3, 10, "Invalid value (Parameter 'value')")]
	[InlineData(5, -1, 6, "Invalid column (Parameter 'col')")]
	[InlineData(-1, 5, 6, "Invalid row (Parameter 'row')")]
	[InlineData(1, 3, -1, "Invalid value (Parameter 'value')")]
	public void SetCell_WhenGivenInvalidNumber_ThrowsInvalidArgumentException(int row, int col, int value, string expectedMessage)
	{
		// Arrange
		var sut = ResolveSut();

		// Act
		void SetCell() => sut.SetCell(row, col, value);

		// Assert
		Assert.Throws<ArgumentException>(SetCell).Message.Should().Be(expectedMessage);
	}

	[Fact]
	public void Reset_ClearsGameState()
	{
		// Arrange
		var mockGameState = Container.ResolveMock<IGameStateMemory>();
		var sut = ResolveSut();
		sut.Restore(PuzzleFactory.GetEmptyPuzzle());

		// Act
		sut.Reset();

		// Assert
		mockGameState.Verify(x => x.Clear(), Times.Once);
	}

	[Fact]
	public void Reset_SetsPuzzle_ToEmptyPuzzle()
	{
		// Arrange
		var sut = ResolveSut();
		sut.Restore(PuzzleFactory.GetSolvedPuzzle());

		// Act
		sut.Reset();

		// Assert
		sut.Puzzle.ToList().ForEach(x => x.Value.Should().BeNull());
	}

	[Fact]
	public void Reset_ResetsScoreBackToZero()
	{
		// Arrange
		var sut = ResolveSut();
		sut.Restore(PuzzleFactory.GetSolvedPuzzle());

		// Act
		sut.Reset();

		// Assert
		sut.Score.Should().Be(0);
	}

	[Theory]
	[InlineData(Level.Easy)]
	[InlineData(Level.Medium)]
	[InlineData(Level.Hard)]
	[InlineData(Level.ExtremelyHard)]
	public void IsValid_WhenGivenValidPuzzle_ReturnsTrue(Level level)
	{
		// Arrange
		var puzzle = PuzzleFactory.GetPuzzle(level);

		// Act
		var isValid = puzzle.IsValid();

		// Assert
		isValid.Should().BeTrue();
	}

	[Fact]
	public void IsValid_WhenGivenEmptyPuzzle_ReturnsTrue()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetEmptyPuzzle();

		// Act
		var isValid = puzzle.IsValid();

		// Assert
		isValid.Should().BeTrue();
	}

	[Fact]
	public void IsValid_WhenGivenCompletedValidPuzzle_ReturnsTrue()
	{
		// Arrange
		var puzzle = PuzzleFactory.GetSolvedPuzzle();

		// Act
		var isValid = puzzle.IsValid();

		// Assert
		isValid.Should().BeTrue();
	}

	[Fact]
	public void IntegrationTest()
	{
		// Arrange
		var strategies = new List<SolverStrategy>
		{
			new ColumnRowMiniGridEliminationStrategy(),
			new LoneRangersInMiniGridsStrategy(),
			new LoneRangersInRowsStrategy(),
			new LoneRangersInColumnsStrategy(),
			new TwinsInMiniGridsStrategy(),
			new TwinsInRowsStrategy(),
			new TwinsInColumnsStrategy(),
			new TripletsInMiniGridsStrategy(),
			new TripletsInRowsStrategy(),
			new TripletsInColumnsStrategy(),
		};
		var puzzleSolver = new PuzzleSolver(strategies);
		var sut = new SudokuGame(new GameStateMemory(), puzzleSolver);
		var puzzle = PuzzleFactory
			.GetPuzzle(Level.ExtremelyHard);
		sut.Restore(puzzle);
		if (!puzzle.IsValid())
		{
			throw new InvalidOperationException();
		}

		// Act
		sut.SolvePuzzle();

		// Assert
		puzzleSolver.IsSolved(sut.Puzzle).Should().BeTrue();
	}
}