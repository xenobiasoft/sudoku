using DepenMock.XUnit;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests;

public class GameStateMemoryTests : BaseTestByType<GameStateMemory>
{
	[Fact]
	public void Push_AddsGameStateToStack()
	{
		// Arrange
		var gameState = new GameStateMemento(Container.Create<string[,]>(), Container.Create<int[,]>(), Container.Create<int>());
		var sut = ResolveSut();

		// Act
		sut.Save(gameState);

		// Assert
		sut.GameState.Count.Should().Be(1);
	}

	[Fact]
	public void Undo_PopsGameStateOffStack()
	{
		// Arrange
		var gameState = new GameStateMemento(Container.Create<string[,]>(), Container.Create<int[,]>(), Container.Create<int>());
		var sut = ResolveSut();

		// Act
		sut.Save(gameState);
		var actualGameState = sut.Undo();

		// Assert
		Assert.Multiple(() =>
		{
			sut.GameState.Count.Should().Be(0);
			actualGameState.Should().Be(gameState);
		});
	}

	[Fact]
	public void Clear_ClearsGameStateStack()
	{
		// Arrange
		var sut = ResolveSut();
		sut.Save(Container.Create<GameStateMemento>());

		// Act
		sut.Clear();

		// Assert
		sut.GameState.Count.Should().Be(0);
	}
}