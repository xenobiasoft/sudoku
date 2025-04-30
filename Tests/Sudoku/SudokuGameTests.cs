using DepenMock.XUnit;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Generator;

namespace UnitTests.Sudoku;

public class SudokuGameTests : BaseTestByAbstraction<SudokuGame, ISudokuGame>
{
    [Fact]
    public async Task DeleteAsync_DeletesGameStateFromStorage()
    {
        // Arrange
        var puzzleId = Container.Create<string>();
        var mockGameStateManager = Container.ResolveMock<IGameStateManager>();
        var sut = ResolveSut();

        // Act
        await sut.DeleteAsync(puzzleId);

        // Assert
        mockGameStateManager.VerifyDeleteAsyncCalled(Times.Once);
    }

    [Fact]
    public async Task LoadAsync_LoadsGameStateFromGameStateManager()
    {
        // Arrange
        var puzzleId = Container.Create<string>();
        var mockGameStateManager = Container.ResolveMock<IGameStateManager>();
        var sut = ResolveSut();

        // Act
        await sut.LoadAsync(puzzleId);

        // Assert
        mockGameStateManager.Verify(x => x.LoadAsync(puzzleId), Times.Once);
    }

    [Fact]
    public async Task NewGameAsync_GeneratesNewPuzzle()
    {
        // Arrange
        var mockPuzzleGenerator = Container.ResolveMock<IPuzzleGenerator>();
        var sut = ResolveSut();

        // Act
        await sut.NewGameAsync(Level.Easy);

        // Assert
        mockPuzzleGenerator.Verify(x => x.Generate(It.IsAny<Level>()), Times.Once);
    }

    [Fact]
    public async Task NewGameAsync_ReturnsPuzzleId()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var puzzleId = await sut.NewGameAsync(Level.Easy);

        // Assert
        puzzleId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SaveAsync_PersistsGameStateToStorage()
    {
        // Arrange
        var mockStorage = Container.ResolveMock<IGameStateManager>();
        var puzzle = Container.Create<ISudokuPuzzle>();
        var sut = ResolveSut();

        // Act
        await sut.SaveAsync(puzzle.ToGameState(0));

        // Assert
        mockStorage.VerifySaveAsyncCalled(Times.Once);
    }
}