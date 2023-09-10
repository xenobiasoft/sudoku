using DepenMock.XUnit;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests;

public class GameStateMemoryTests : BaseTestByAbstraction<GameStateMemory, IGameStateMemory>
{
	[Fact]
	public void Push_AddsGameStateToStack()
	{
		// Arrange
		var gameState = new GameStateMemento(Container.CreateMany<Cell>(), Container.Create<int>());
		var sut = ResolveSut();

		// Act
		sut.Save(gameState);

		// Assert
		sut.IsEmpty().Should().BeFalse();
	}

	[Fact]
	public void Undo_PopsGameStateOffStack()
	{
		// Arrange
		var gameState = new GameStateMemento(Container.CreateMany<Cell>(), Container.Create<int>());
		var sut = ResolveSut();

		// Act
		sut.Save(gameState);
		var actualGameState = sut.Undo();

		// Assert
		Assert.Multiple(() =>
		{
			sut.IsEmpty().Should().BeTrue();
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
		sut.IsEmpty().Should().BeTrue();
	}
}