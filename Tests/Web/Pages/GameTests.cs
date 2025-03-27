using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.Pages;
using Sudoku.Web.Server.Services;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;

namespace UnitTests.Web.Pages;

public class GameTests : TestContext
{
    private readonly Mock<ISudokuGame> _mockSudokuGame = new();
    private readonly Mock<ICellFocusedNotificationService> _mockCellFocusedNotifier = new();
    private readonly Mock<IInvalidCellNotificationService> _mockInvalidCellNotifier = new();
    private readonly Mock<IGameNotificationService> _mockGameNotificationService = new();

    public GameTests()
    {
        _mockSudokuGame
            .Setup(x => x.Puzzle)
            .Returns(PuzzleFactory.GetPuzzle(Level.Easy));

        Services.AddSingleton(_mockInvalidCellNotifier.Object);
        Services.AddSingleton(_mockCellFocusedNotifier.Object);
        Services.AddSingleton(_mockGameNotificationService.Object);
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

    [Fact]
    public async Task Game_WhenInvalidNumberIsEntered_HighlightsInvalidCells()
    {
        // Arrange
        var cell = _mockSudokuGame.Object.Puzzle.GetCell(0, 2);
        var game = RenderComponent<Game>();
        var cellInput = game.FindComponents<CellInput>().FirstOrDefault(x => x.Instance.Cell == cell)!.Instance;
        await game.InvokeAsync(() => cellInput.OnCellFocus.InvokeAsync(cell));
        var buttonGroup = game.FindComponent<ButtonGroup>().Instance;

        // Act
        await game.InvokeAsync(() => buttonGroup.NumberClicked.InvokeAsync(5));

        // Assert
        _mockInvalidCellNotifier.Verify(x => x.Notify(It.IsAny<IEnumerable<Cell>>()), Times.Once);
    }

    [Fact]
    public void Game_WhenNewGame_SendsGameStartedNotification()
    {
        // Arrange

        // Act
        RenderComponent<Game>();

        // Assert
        _mockGameNotificationService.Verify(x => x.NotifyGameStarted(), Times.Once);
    }

    [Fact]
    public async Task Game_WhenPuzzleSolved_SendsGameEndedNotification()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetSolvedPuzzle();
        var cell = puzzle.GetCell(0, 0);
        cell.Value = null;
        _mockSudokuGame
            .Setup(x => x.Puzzle)
            .Returns(puzzle);
        var game = RenderComponent<Game>();
        var cellInput = game.FindComponents<CellInput>().FirstOrDefault(x => x.Instance.Cell == cell)!.Instance;
        await game.InvokeAsync(() => cellInput.OnCellFocus.InvokeAsync(cell));
        var buttonGroup = game.FindComponent<ButtonGroup>().Instance;

        // Act
        await game.InvokeAsync(() => buttonGroup.NumberClicked.InvokeAsync(1));

        // Assert
        _mockGameNotificationService.Verify(x => x.NotifyGameEnded(), Times.Once);
    }

    [Fact]
    public async Task Game_WhenPuzzleSolved_DisplaysWinScreen()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetSolvedPuzzle();
        var cell = puzzle.GetCell(0, 0);
        cell.Value = null;
        _mockSudokuGame
            .Setup(x => x.Puzzle)
            .Returns(puzzle);
        var game = RenderComponent<Game>();
        var cellInput = game.FindComponents<CellInput>().FirstOrDefault(x => x.Instance.Cell == cell)!.Instance;
        await game.InvokeAsync(() => cellInput.OnCellFocus.InvokeAsync(cell));
        var buttonGroup = game.FindComponent<ButtonGroup>().Instance;

        // Act
        await game.InvokeAsync(() => buttonGroup.NumberClicked.InvokeAsync(1));

        // Assert
        var victoryOverlay = game.Find(".victory-overlay");
        Assert.NotNull(victoryOverlay);
    }
}