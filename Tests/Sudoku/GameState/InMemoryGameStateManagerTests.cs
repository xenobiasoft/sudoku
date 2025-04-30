using DepenMock.XUnit;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Sudoku.GameState;

public class InMemoryGameStateManagerTests : BaseTestByAbstraction<InMemoryGameStateManager, IGameStateManager>
{
    private const string PuzzleId = "test-puzzle";

    [Fact]
    public async Task ClearAsync_ShouldClearGameState()
    {
        // Arrange
        var sut = ResolveSut();

        await sut.SaveAsync(Container.Create<GameStateMemory>());

        // Act
        await sut.DeleteAsync(PuzzleId);

        // Assert
        var result = await sut.LoadAsync(PuzzleId);
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoadAsync_ShouldReturnNull_WhenNoGameStateExists()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var result = await sut.LoadAsync(PuzzleId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoadAsync_ShouldReturnLastSavedGameState()
    {
        // Arrange
        var expectedGameState = Container.Create<GameStateMemory>();
        var sut = ResolveSut();

        await sut.SaveAsync(expectedGameState);

        // Act
        var actualGameState = await sut.LoadAsync(PuzzleId);

        // Assert
        actualGameState.Should().Be(expectedGameState);
    }

    [Fact]
    public async Task SaveAsync_ShouldNotSaveDuplicateGameState()
    {
        // Arrange
        var board = new[] { new Cell(0, 0) { Value = 1 } };
        var gameState1 = new GameStateMemory(PuzzleId, board, 0);
        var gameState2 = new GameStateMemory(PuzzleId, board, 0);
        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(gameState1);
        await sut.SaveAsync(gameState2);

        // Assert
        var result = await sut.LoadAsync(PuzzleId);

        result.Should().Be(gameState1);
    }

    [Fact]
    public async Task UndoAsync_ShouldReturnLastGameStateAndRemoveIt()
    {
        // Arrange
        var gameState1 = new GameStateMemory(PuzzleId, [], 0);
        var gameState2 = new GameStateMemory(PuzzleId, [], 1);
        var sut = ResolveSut();

        await sut.SaveAsync(gameState1);
        await sut.SaveAsync(gameState2);

        // Act
        var undoneState = await sut.UndoAsync(PuzzleId);
        var currentState = await sut.LoadAsync(PuzzleId);

        // Assert
        Assert.Multiple(() =>
        {
            undoneState.Should().Be(gameState2);
            currentState.Should().Be(gameState1);
        });
    }

    [Fact]
    public async Task UndoAsync_ShouldReturnNull_WhenNoGameStateExists()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var result = await sut.UndoAsync(PuzzleId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GameStateMemoryType_ShouldBeInMemory()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var type = sut.MemoryType;

        // Assert
        type.Should().Be(GameStateMemoryType.InMemory);
    }
}