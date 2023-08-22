using DepenMock.XUnit;
using Newtonsoft.Json.Linq;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.PuzzleSolver;
using XenobiaSoft.Sudoku.Strategies;

namespace UnitTests;

public class SudokuGameTests : BaseTestByAbstraction<SudokuGame, ISudokuGame>
{
	private readonly SudokuPuzzle _puzzle;

	public SudokuGameTests()
	{
		_puzzle = PuzzleFactory
			.GetPuzzle(Level.ExtremelyHard)
			.PopulatePossibleValues();

		Container.Register(_puzzle);
	}

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
	public void Undo_RestoresGameStateFromSavedGameState()
	{
		// Arrange
		var gameState = Container.Create<GameStateMemento>();
		Container
			.ResolveMock<IGameStateMemory>()
			.Setup(x => x.Undo())
			.Returns(gameState);
		var sut = ResolveSut();

		// Act
		sut.Undo();

		// Assert
		Assert.Multiple(() =>
		{
			Assert.Equal(gameState.Score, sut.Score);
			Assert.Equal(gameState.PossibleValues, sut.Puzzle.PossibleValues);
			Assert.Equal(gameState.Values, sut.Puzzle.Values);
		});
	}

	[Fact]
	public void LoadPuzzle_SetsGamePuzzle()
	{
		// Arrange
		var puzzle = Container.Create<SudokuPuzzle>();
		var sut = ResolveSut();

		// Act
		sut.LoadPuzzle(puzzle);

		// Assert
		Assert.Multiple(() =>
		{
			Assert.Equal(puzzle.PossibleValues, sut.Puzzle.PossibleValues);
			Assert.Equal(puzzle.Values, sut.Puzzle.Values);
		});
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
		mockPuzzleSolver.Verify(x => x.TrySolvePuzzle(It.IsAny<SudokuPuzzle>()), Times.Once);
	}

	[Fact]
	public void SolvePuzzle_SetsScoreBasedOnPuzzleSolverScore()
	{
		// Arrange
		const int expectedScore = 45;
		Container
			.ResolveMock<IPuzzleSolver>()
			.Setup(x => x.TrySolvePuzzle(It.IsAny<SudokuPuzzle>()))
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
			.SetupSequence(x => x.IsSolved(It.IsAny<SudokuPuzzle>()))
			.Returns(false)
			.Returns(true);
		Container
			.ResolveMock<IPuzzleSolver>()
			.Setup(x => x.TrySolvePuzzle(It.IsAny<SudokuPuzzle>()))
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
		var expectedGameState = Container.Create<GameStateMemento>();
		Container
			.ResolveMock<IGameStateMemory>()
			.Setup(x => x.Undo())
			.Returns(expectedGameState);
		Container
			.ResolveMock<IPuzzleSolver>()
			.Setup(x => x.TrySolvePuzzle(It.IsAny<SudokuPuzzle>()))
			.Throws<InvalidOperationException>();
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle();

		// Assert
		Assert.Multiple(() =>
		{
			Assert.Equal(expectedGameState.PossibleValues, sut.Puzzle.PossibleValues);
			Assert.Equal(expectedGameState.Values, sut.Puzzle.Values);
			Assert.Equal(expectedGameState.Score, sut.Score);
		});
	}

	[Fact]
	public void SolvePuzzle_WhenUsingBruteForceMethod_SetsCellWithFewestPossibleValues()
	{
		// Arrange
		Container
			.ResolveMock<IPuzzleSolver>()
			.SetupSequence(x => x.IsSolved(It.IsAny<SudokuPuzzle>()))
			.Returns(false)
			.Returns(true);
		Container
			.ResolveMock<IPuzzleSolver>()
			.Setup(x => x.TrySolvePuzzle(It.IsAny<SudokuPuzzle>()))
			.Returns(0);
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle();

		// Assert
		_puzzle.Values[3, 5].Should().BeOneOf(5, 9);
	}

	[Fact]
	public void SolvePuzzle_WhenUsingBruteForceMethod_AddsFivePointsToExistingScore()
	{
		// Arrange
		Container
			.ResolveMock<IPuzzleSolver>()
			.SetupSequence(x => x.IsSolved(It.IsAny<SudokuPuzzle>()))
			.Returns(false)
			.Returns(false)
			.Returns(true);
		var puzzleSolverScore = 45;
		Container
			.ResolveMock<IPuzzleSolver>()
			.SetupSequence(x => x.TrySolvePuzzle(It.IsAny<SudokuPuzzle>()))
			.Returns(puzzleSolverScore)
			.Returns(0);
		var sut = ResolveSut();
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
			.Setup(x => x.IsSolved(It.IsAny<SudokuPuzzle>()))
			.Returns(false);
		mockPuzzleSolver
			.Setup(x => x.TrySolvePuzzle(It.IsAny<SudokuPuzzle>()))
			.Returns(0);
		var sut = ResolveSut();

		// Act
		sut.SolvePuzzle();

		// Assert
		mockPuzzleSolver.Verify(x => x.TrySolvePuzzle(It.IsAny<SudokuPuzzle>()), Times.Exactly(50));
	}

	[Fact]
	public void SetCell_AcceptsValue_AndSetsCell()
	{
		// Arrange
		var sut = ResolveSut();

		// Act
		sut.SetCell(2, 1, 5);

		// Assert
		sut.Puzzle.Values[2, 1].Should().Be(5);
	}

	[Theory]
	[InlineData(9, 5, 6, "Invalid column (Parameter 'col')")]
	[InlineData(5, 9, 6, "Invalid row (Parameter 'row')")]
	[InlineData(3, 1, 10, "Invalid value (Parameter 'value')")]
	[InlineData(-1, 5, 6, "Invalid column (Parameter 'col')")]
	[InlineData(5, -1, 6, "Invalid row (Parameter 'row')")]
	[InlineData(3, 1, -1, "Invalid value (Parameter 'value')")]
	public void SetCell_WhenGivenInvalidNumber_ThrowsInvalidArgumentException(int col, int row, int value, string expectedMessage)
	{
		// Arrange
		var sut = ResolveSut();

		// Act
		void SetCell() => sut.SetCell(col, row, value);

		// Assert
		Assert.Throws<ArgumentException>(SetCell).Message.Should().Be(expectedMessage);
	}

	//[Fact]
	public void IntegrationTest()
	{
		// Arrange
		var strategies = new List<SolverStrategy>
		{
			new ColumnRowMiniGridEliminationStrategy(),
			new LoneRangersInColumnsStrategy(),
			new LoneRangersInRowsStrategy(),
			new LoneRangersInMiniGridsStrategy(),
			new TwinsInColumnsStrategy(),
			new TwinsInRowsStrategy(),
			new TwinsInMiniGridsStrategy(),
			new TripletsInColumnsStrategy(),
			new TripletsInRowsStrategy(),
			new TripletsInMiniGridsStrategy()
		};
		var puzzleSolver = new PuzzleSolver(strategies);
		var sut = new SudokuGame(new GameStateMemory(), puzzleSolver);
		var puzzle = PuzzleFactory
			.GetPuzzle(Level.ExtremelyHard)
			.PopulatePossibleValues();
		sut.LoadPuzzle(puzzle);

		// Act
		sut.SolvePuzzle();

		// Assert
		puzzleSolver.IsSolved(sut.Puzzle).Should().BeTrue();
	}
}