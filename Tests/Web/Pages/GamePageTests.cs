using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.EventArgs;
using Sudoku.Web.Server.Pages;
using Sudoku.Web.Server.Services;
using UnitTests.Helpers;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Web.Pages;

public class GamePageTests : TestContext
{
    private readonly Mock<IInvalidCellNotificationService> _mockInvalidCellNotifier = new();
    private readonly Mock<IGameNotificationService> _mockGameNotificationService = new();
    private readonly Mock<IGameStateManager> _mockGameStateManager = new();
    private readonly Mock<IGameSessionManager> _mockGameSessionManager = new();

    public GamePageTests()
    {
        var loadedGameState = new GameStateMemory("puzzle1", PuzzleFactory.GetPuzzle(Level.Easy).GetAllCells())
        {
            InvalidMoves = 0,
            LastResumeTime = DateTime.UtcNow,
            PlayDuration = TimeSpan.FromMinutes(10),
            StartTime = DateTime.UtcNow.AddMinutes(-10),
            TotalMoves = 5
        };
        _mockGameStateManager.SetupLoadGameAsync(loadedGameState);
        _mockGameSessionManager.Setup(x => x.CurrentSession).Returns(new GameSession(loadedGameState, new Mock<IGameTimer>().Object));
        
        Services.AddSingleton(_mockInvalidCellNotifier.Object);
        Services.AddSingleton(_mockGameNotificationService.Object);
        Services.AddSingleton(new Mock<ICellFocusedNotificationService>().Object);
        Services.AddSingleton(new Mock<ISudokuPuzzle>().Object);
        Services.AddSingleton(_mockGameSessionManager.Object);
        Services.AddSingleton(_mockGameStateManager.Object);
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
        await sut.InvokeAsync(() => buttonGroup.OnNumberClicked.InvokeAsync(new CellValueChangedEventArgs(3)));

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
        await sut.InvokeAsync(() => buttonGroup.OnNumberClicked.InvokeAsync(new CellValueChangedEventArgs(5)));

        // Assert
        _mockInvalidCellNotifier.VerifyNotificationSent(Times.Once);
    }

    [Fact]
    public void Game_WhenPuzzleLoaded_SendsGameStartedNotification()
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
        var gameState = new GameStateMemory("puzzle1", PuzzleFactory.GetSolvedPuzzle().GetAllCells());
        _mockGameStateManager.SetupLoadGameAsync(gameState);
        var sut = RenderComponent<Game>();
        var buttonGroup = sut.FindComponent<ButtonGroup>().Instance;
        var cellInput = sut.FindComponent<CellInput>().Instance;

        // Act
        await sut.InvokeAsync(() => cellInput.OnCellFocus.InvokeAsync(cellInput.Cell));
        await sut.InvokeAsync(() => buttonGroup.OnNumberClicked.InvokeAsync(new CellValueChangedEventArgs(1)));

        // Assert
        _mockGameNotificationService.VerifyGameEndedSent(Times.Once);
    }

    [Fact]
    public async Task Game_WhenPuzzleSolved_DisplaysWinScreen()
    {
        // Arrange
        var gameState = new GameStateMemory("puzzle1", PuzzleFactory.GetSolvedPuzzle().GetAllCells());
        _mockGameStateManager.SetupLoadGameAsync(gameState);
        var sut = RenderComponent<Game>();
        var buttonGroup = sut.FindComponent<ButtonGroup>().Instance;
        var cellInput = sut.FindComponent<CellInput>().Instance;

        // Act
        await sut.InvokeAsync(() => cellInput.OnCellFocus.InvokeAsync(cellInput.Cell));
        await sut.InvokeAsync(() => buttonGroup.OnNumberClicked.InvokeAsync(new CellValueChangedEventArgs(1)));

        // Assert
        var victoryOverlay = sut.Find(".victory-overlay");
        victoryOverlay.Should().NotBeNull();
    }

    [Fact]
    public async Task Game_WhenPuzzleSolved_DeletesGame()
    {
        // Arrange
        var gameState = new GameStateMemory("puzzle1", PuzzleFactory.GetSolvedPuzzle().GetAllCells());
        _mockGameStateManager.SetupLoadGameAsync(gameState);
        var sut = RenderComponent<Game>();
        var buttonGroup = sut.FindComponent<ButtonGroup>().Instance;
        var cellInput = sut.FindComponent<CellInput>().Instance;

        // Act
        await sut.InvokeAsync(() => cellInput.OnCellFocus.InvokeAsync(cellInput.Cell));
        await sut.InvokeAsync(() => buttonGroup.OnNumberClicked.InvokeAsync(new CellValueChangedEventArgs(1)));

        // Assert
        _mockGameStateManager.VerifyDeleteGameAsyncCalled(sut.Instance.PuzzleId, Times.Once);
    }

    [Fact]
    public void OnInitializedAsync_LoadsGameState()
    {
        // Arrange

        // Act
        RenderComponent<Game>(parameters => parameters.Add(p => p.PuzzleId, "puzzle1"));

        // Assert
        _mockGameStateManager.VerifyLoadsAsync("puzzle1", Times.Once);
    }

    [Fact]
    public async Task HandleCellChanged_WhenGameBoardRaisesChangedEvent_ReceivesExpectedArgs()
    {
        // Arrange
        CellChangedEventArgs? actualArgs = null;
        var expectedArgs = new CellChangedEventArgs(1, 2, 5);
        var game = RenderComponent<Game>();
        var gameBoard = game.FindComponent<GameBoard>();
        void OnCellChangedHandler(CellChangedEventArgs args) => actualArgs = args;
        gameBoard.Instance.OnCellChanged = EventCallback.Factory.Create<CellChangedEventArgs>(this, OnCellChangedHandler);


        // Act
        await game.InvokeAsync(() =>
            gameBoard.Instance.OnCellChanged.InvokeAsync(expectedArgs)
        );

        // Assert
        actualArgs.Should().BeEquivalentTo(expectedArgs);
    }

    [Fact]
    public async Task HandleUndo_ShouldLoadPreviousGameStateAndUpdatePuzzle()
    {
        // Arrange
        var puzzleId = "test-puzzle";
        var gameState = new GameStateMemory(puzzleId, []);
        _mockGameStateManager.Setup(x => x.UndoAsync(puzzleId)).ReturnsAsync(gameState);
        var game = RenderComponent<Game>(parameters => parameters.Add(p => p.PuzzleId, puzzleId));

        // Act
        await game.InvokeAsync(() => game.Instance.HandleUndo());

        // Assert
        _mockGameStateManager.Verify(x => x.UndoAsync(puzzleId), Times.Once);
    }
}