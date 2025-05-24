using DepenMock.XUnit;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Extensions;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Sudoku.GameState;

public class InMemoryGameStateStorageTests : BaseTestByAbstraction<InMemoryGameStateStorage, IGameStateStorage<PuzzleState>>
{
    private const string PuzzleId = "test-puzzle";
    private const string Alias = "test-alias";

    [Fact]
    public async Task ClearAsync_ShouldClearGameState()
    {
        // Arrange
        var sut = ResolveSut();

        await sut.SaveAsync(Container.Create<PuzzleState>());

        // Act
        await sut.DeleteAsync(Alias, PuzzleId);

        // Assert
        var result = await sut.LoadAsync(Alias, PuzzleId);
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoadAsync_WhenNoGameStateExists_ShouldReturnNull()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var result = await sut.LoadAsync(Alias, PuzzleId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoadAsync_ShouldReturnLastSavedGameState()
    {
        // Arrange
        var expectedGameState = Container.Create<PuzzleState>();
        var sut = ResolveSut();

        await sut.SaveAsync(expectedGameState);

        // Act
        var actualGameState = await sut.LoadAsync(Alias, PuzzleId);

        // Assert
        actualGameState.Should().Be(expectedGameState);
    }

    [Fact]
    public void PuzzleStateType_ShouldBeInMemory()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var type = sut.MemoryType;

        // Assert
        type.Should().Be(GameStateMemoryType.InMemory);
    }

    [Fact]
    public async Task ResetAsync_ShouldRemoveAllButInitialGameState()
    {
        // Arrange
        var initialGameState = new PuzzleState(PuzzleId, []);
        var sut = ResolveSut();
        await sut.SaveAsync(initialGameState);

        for (var i = 0; i < 4; i++)
        {
            await sut.SaveAsync(PuzzleFactory.GetPuzzle(Level.Easy).ToPuzzleState());
        }

        // Act
        var result = await sut.ResetAsync(Alias, PuzzleId);

        // Assert
        result.Should().BeEquivalentTo(initialGameState);
    }

    [Fact]
    public async Task SaveAsync_ShouldNotSaveDuplicateGameState()
    {
        // Arrange
        var board = new[] { new Cell(0, 0) { Value = 1 } };
        var gameState1 = new PuzzleState(PuzzleId, board);
        var gameState2 = new PuzzleState(PuzzleId, board);
        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(gameState1);
        await sut.SaveAsync(gameState2);

        // Assert
        var result = await sut.LoadAsync(Alias, PuzzleId);

        result.Should().Be(gameState1);
    }

    [Fact]
    public async Task UndoAsync_ShouldReturnLastGameStateAndRemoveIt()
    {
        // Arrange
        var gameState1 = PuzzleFactory.GetPuzzle(Level.Easy).ToPuzzleState();
        var gameState2 = PuzzleFactory.GetPuzzle(Level.Easy).ToPuzzleState();
        var sut = ResolveSut();

        await sut.SaveAsync(gameState1);
        await sut.SaveAsync(gameState2);

        // Act
        var undoneState = await sut.UndoAsync(Alias, PuzzleId);
        var currentState = await sut.LoadAsync(Alias, PuzzleId);

        // Assert
        Assert.Multiple(() =>
        {
            undoneState!.AssertAreEquivalent(gameState2);
            currentState!.AssertAreEquivalent(gameState1);
        });
    }

    [Fact]
    public async Task UndoAsync_WhenNoGameStateExists_ShouldReturnNull()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var result = await sut.UndoAsync(Alias, PuzzleId);

        // Assert
        result.Should().BeNull();
    }
}