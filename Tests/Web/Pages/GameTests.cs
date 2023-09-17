using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.Pages;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;

namespace UnitTests.Web.Pages;

public class GameTests : TestContext
{
    private readonly Mock<ISudokuGame> _mockSudokuGame = new();

    public GameTests()
    {
        _mockSudokuGame
            .Setup(x => x.Puzzle)
            .Returns(PuzzleFactory.GetPuzzle(Level.Easy));

        Services.AddTransient(x => _mockSudokuGame.Object);
    }

    [Fact]
    public async Task Game_WhenDifficultyLevelChanges_RendersNewGame()
    {
        // Arrange
        var difficultyLevel = Level.Hard;
        var game = RenderComponent<Game>();
        var difficultyLevelComponent = game.FindComponent<DifficultyLevelOptions>();

        // Act
        await game.InvokeAsync(() => difficultyLevelComponent.Instance.OnDifficultyLevelChanged.InvokeAsync(difficultyLevel));

        // Assert
        _mockSudokuGame.Verify(x => x.New(difficultyLevel), Times.Once);
    }

    [Fact]
    public async Task Game_WhenButtonGroupClicked_SetsCellValue()
    {
        // Arrange
        var cell = _mockSudokuGame.Object.Puzzle.GetCell(0, 0);
        var game = RenderComponent<Game>();
        var cellInput = game.FindComponent<CellInput>().Instance;
        await game.InvokeAsync(() => cellInput.OnCellFocus.InvokeAsync(cell));
        var buttonGroup = game.FindComponent<ButtonGroup>().Instance;

        // Act
        await game.InvokeAsync(() => buttonGroup.NumberClicked.InvokeAsync(3));

        // Assert
        cellInput.Cell.Value.Should().Be(3);
    }
}