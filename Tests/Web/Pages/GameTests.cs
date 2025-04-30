using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.Pages;
using Sudoku.Web.Server.Services;
using UnitTests.Helpers;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku;

namespace UnitTests.Web.Pages;

public class GameTests : TestContext
{
    private readonly Mock<IInvalidCellNotificationService> _mockInvalidCellNotifier = new();
    private readonly Mock<IGameNotificationService> _mockGameNotificationService = new();
    private readonly Mock<ICellFocusedNotificationService> _mockCellFocusedNotificationService = new();
    private readonly Mock<ISudokuPuzzle> _mockPuzzle = new();
    private readonly Mock<ISudokuGame> _mockGame = new();

    public GameTests()
    {
        _mockGame.SetPuzzle(PuzzleFactory.GetPuzzle(Level.Easy));

        Services.AddSingleton(_mockInvalidCellNotifier.Object);
        Services.AddSingleton(_mockGameNotificationService.Object);
        Services.AddSingleton(_mockCellFocusedNotificationService.Object);
        Services.AddSingleton(_mockPuzzle.Object);
        Services.AddSingleton(_mockGame.Object);
    }

    [Fact]
    public async Task Game_WhenButtonGroupClicked_SetsCellValue()
    {
        // Arrange
        var sut = RenderComponent<Game>();
        var buttonGroup = sut.FindComponent<ButtonGroup>().Instance;
        var cellInput = sut.FindComponent<CellInput>().Instance;

        // Act
        await sut.InvokeAsync(() => cellInput.OnCellFocus.InvokeAsync(cellInput.Cell));
        await sut.InvokeAsync(() => buttonGroup.NumberClicked.InvokeAsync(3));

        // Assert
        cellInput.Cell.Value.Should().Be(3);
    }

    [Fact]
    public async Task Game_WhenInvalidNumberIsEntered_HighlightsInvalidCells()
    {
        // Arrange
        var sut = RenderComponent<Game>();
        var buttonGroup = sut.FindComponent<ButtonGroup>().Instance;
        var cellInput = sut.FindComponent<CellInput>().Instance;

        // Act
        await sut.InvokeAsync(() => cellInput.OnCellFocus.InvokeAsync(cellInput.Cell));
        await sut.InvokeAsync(() => buttonGroup.NumberClicked.InvokeAsync(5));

        // Assert
        _mockInvalidCellNotifier.Verify(x => x.Notify(It.IsAny<IEnumerable<Cell>>()), Times.Once);
    }

    [Fact]
    public async Task Game_WhenPuzzleSolved_SendsGameEndedNotification()
    {
        // Arrange
        _mockGame.SetPuzzle(PuzzleFactory.GetSolvedPuzzle());
        var sut = RenderComponent<Game>();
        var buttonGroup = sut.FindComponent<ButtonGroup>().Instance;
        var cellInput = sut.FindComponent<CellInput>().Instance;

        // Act
        await sut.InvokeAsync(() => cellInput.OnCellFocus.InvokeAsync(cellInput.Cell));
        await sut.InvokeAsync(() => buttonGroup.NumberClicked.InvokeAsync(1));

        // Assert
        _mockGameNotificationService.Verify(x => x.NotifyGameEnded(), Times.Once);
    }

    [Fact]
    public async Task Game_WhenPuzzleSolved_DisplaysWinScreen()
    {
        // Arrange
        _mockGame.SetPuzzle(PuzzleFactory.GetSolvedPuzzle());
        var sut = RenderComponent<Game>();
        var buttonGroup = sut.FindComponent<ButtonGroup>().Instance;
        var cellInput = sut.FindComponent<CellInput>().Instance;

        // Act
        await sut.InvokeAsync(() => cellInput.OnCellFocus.InvokeAsync(cellInput.Cell));
        await sut.InvokeAsync(() => buttonGroup.NumberClicked.InvokeAsync(1));

        // Assert
        var victoryOverlay = sut.Find(".victory-overlay");
        victoryOverlay.Should().NotBeNull();
    }
}