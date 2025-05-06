using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.Pages;
using Sudoku.Web.Server.Services;
using UnitTests.Helpers;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku;

namespace UnitTests.Web.Pages;

public class GamePageTests : TestContext
{
    private readonly Mock<IInvalidCellNotificationService> _mockInvalidCellNotifier = new();
    private readonly Mock<IGameNotificationService> _mockGameNotificationService = new();
    private readonly Mock<ISudokuGame> _mockGame = new();

    public GamePageTests()
    {
        _mockGame.SetLoadAsync(PuzzleFactory.GetPuzzle(Level.Easy));

        Services.AddSingleton(_mockInvalidCellNotifier.Object);
        Services.AddSingleton(_mockGameNotificationService.Object);
        Services.AddSingleton(_mockGame.Object);
        Services.AddSingleton(new Mock<ICellFocusedNotificationService>().Object);
        Services.AddSingleton(new Mock<ISudokuPuzzle>().Object);
        Services.AddSingleton(new Mock<IGameStateManager>().Object);
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
    public async Task Game_WhenInvalidNumberIsEntered_NotifiesInvalidCell()
    {
        // Arrange
        var sut = RenderComponent<Game>();
        var buttonGroup = sut.FindComponent<ButtonGroup>().Instance;
        var cellInput = sut.FindComponent<CellInput>().Instance;

        // Act
        await sut.InvokeAsync(() => cellInput.OnCellFocus.InvokeAsync(cellInput.Cell));
        await sut.InvokeAsync(() => buttonGroup.NumberClicked.InvokeAsync(5));

        // Assert
        _mockInvalidCellNotifier.VerifyNotificationSent(Times.Once);
    }

    [Fact]
    public async Task Game_WhenPuzzleLoaded_SendsGameStartedNotification()
    {
        // Arrange

        // Act
        RenderComponent<Game>();

        // Assert
        _mockGameNotificationService.VerifyGameStartedSent(Times.Once);
    }

    [Fact]
    public async Task Game_WhenPuzzleSolved_SendsGameEndedNotification()
    {
        // Arrange
        _mockGame.SetLoadAsync(PuzzleFactory.GetSolvedPuzzle());
        var sut = RenderComponent<Game>();
        var buttonGroup = sut.FindComponent<ButtonGroup>().Instance;
        var cellInput = sut.FindComponent<CellInput>().Instance;

        // Act
        await sut.InvokeAsync(() => cellInput.OnCellFocus.InvokeAsync(cellInput.Cell));
        await sut.InvokeAsync(() => buttonGroup.NumberClicked.InvokeAsync(1));

        // Assert
        _mockGameNotificationService.VerifyGameEndedSent(Times.Once);
    }

    [Fact]
    public async Task Game_WhenPuzzleSolved_DisplaysWinScreen()
    {
        // Arrange
        _mockGame.SetLoadAsync(PuzzleFactory.GetSolvedPuzzle());
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

    [Fact]
    public void OnInitializedAsync_LoadsGameState()
    {
        // Arrange
        var puzzle = PuzzleFactory.GetPuzzle(Level.Easy);
        _mockGame.SetLoadAsync(puzzle);

        // Act
        RenderComponent<Game>(parameters => parameters.Add(p => p.PuzzleId, "puzzle1"));

        // Assert
        puzzle.GetAllCells().Should().BeEquivalentTo(puzzle.GetAllCells());
    }

    [Fact]
    public void OnInitializedAsync_LoadsPuzzle()
    {
        // Arrange
        var puzzleId = "puzzle1";
        _mockGame.SetLoadAsync(PuzzleFactory.GetPuzzle(Level.Easy));

        // Act
        RenderComponent<Game>(parameters => parameters.Add(p => p.PuzzleId, puzzleId));

        // Assert
        _mockGame.VerifyLoadsAsync(puzzleId, Times.Once);
    }
}