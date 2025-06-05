using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.EventArgs;
using Sudoku.Web.Server.Pages;
using Sudoku.Web.Server.Services;
using Sudoku.Web.Server.Services.Abstractions;
using UnitTests.Helpers;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Web.Pages;

public class GamePageTests : TestContext
{
    private const string Alias = "test-alias";
    private const string PuzzleId = "test-puzzleId";

    private readonly Mock<IInvalidCellNotificationService> _mockInvalidCellNotifier = new();
    private readonly Mock<IGameNotificationService> _mockGameNotificationService = new();
    private readonly Mock<IGameStateManager> _mockGameStateManager = new();
    private readonly Mock<IGameSessionManager> _mockGameSessionManager = new();
    private readonly Mock<IAliasService> _mockAliasService = new();

    public GamePageTests()
    {
        var loadedGameState = new GameStateMemory(PuzzleId, PuzzleFactory.GetPuzzle(Level.Easy).GetAllCells())
        {
            Alias = Alias,
            InvalidMoves = 0,
            PlayDuration = TimeSpan.FromMinutes(10),
            TotalMoves = 5
        };
        _mockGameStateManager.SetupLoadGameAsync(loadedGameState);
        _mockAliasService.Setup(x => x.GetAliasAsync()).ReturnsAsync(Alias);
        _mockGameSessionManager.Setup(x => x.CurrentSession).Returns(new GameSession(loadedGameState, new Mock<IGameTimer>().Object));
        var mockGameSession = new Mock<IGameSession>();
        var mockGameTimer = new Mock<IGameTimer>();
        mockGameSession.Setup(x => x.Timer).Returns(mockGameTimer.Object);
        Services.AddSingleton(_mockInvalidCellNotifier.Object);
        Services.AddSingleton(_mockGameNotificationService.Object);
        Services.AddSingleton(new Mock<ICellFocusedNotificationService>().Object);
        Services.AddSingleton(new Mock<ISudokuPuzzle>().Object);
        Services.AddSingleton(mockGameSession.Object);
        Services.AddSingleton(_mockGameSessionManager.Object);
        Services.AddSingleton(_mockGameStateManager.Object);
        Services.AddSingleton(_mockAliasService.Object);
    }

    [Fact]
    public async Task Game_WhenButtonGroupClicked_SetsCellValue()
    {
        // Arrange
        var sut = RenderComponent<Game>();
        var buttonGroup = sut.FindComponent<GameControls>().Instance;
        var cellInput = sut.FindComponent<CellInput>().Instance;

        // Act
        await sut.InvokeAsync(() => cellInput.OnCellFocus.InvokeAsync(cellInput.Cell));
        await sut.InvokeAsync(() => buttonGroup.OnValueChanged.InvokeAsync(new CellValueChangedEventArgs(3)));

        // Assert
        cellInput.Cell.Value.Should().Be(3);
    }

    [Fact]
    public async Task Game_WhenInvalidNumberIsEntered_NotifiesInvalidCell()
    {
        // Arrange
        var sut = RenderComponent<Game>();
        var buttonGroup = sut.FindComponent<GameControls>().Instance;
        var cellInput = sut.FindComponent<CellInput>().Instance;

        // Act
        await sut.InvokeAsync(() => cellInput.OnCellFocus.InvokeAsync(cellInput.Cell));
        await sut.InvokeAsync(() => buttonGroup.OnValueChanged.InvokeAsync(new CellValueChangedEventArgs(5)));

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
        var gameState = new GameStateMemory(PuzzleId, PuzzleFactory.GetSolvedPuzzle().GetAllCells())
        {
            Alias = Alias
        };
        _mockGameStateManager.SetupLoadGameAsync(gameState);
        var sut = RenderComponent<Game>(x => x.Add(p => p.PuzzleId, PuzzleId));
        var buttonGroup = sut.FindComponent<GameControls>().Instance;
        var cellInput = sut.FindComponent<CellInput>().Instance;

        // Act
        await sut.InvokeAsync(() => cellInput.OnCellFocus.InvokeAsync(cellInput.Cell));
        await sut.InvokeAsync(() => buttonGroup.OnValueChanged.InvokeAsync(new CellValueChangedEventArgs(1)));

        // Assert
        _mockGameNotificationService.VerifyGameEndedSent(Times.Once);
    }

    [Fact]
    public async Task Game_WhenPuzzleSolved_DisplaysWinScreen()
    {
        // Arrange
        var gameState = new GameStateMemory(PuzzleId, PuzzleFactory.GetSolvedPuzzle().GetAllCells())
        {
            Alias = Alias
        };
        _mockGameStateManager.SetupLoadGameAsync(gameState);
        var sut = RenderComponent<Game>(x => x.Add(p => p.PuzzleId, PuzzleId));
        var buttonGroup = sut.FindComponent<GameControls>().Instance;
        var cellInput = sut.FindComponent<CellInput>().Instance;

        // Act
        await sut.InvokeAsync(() => cellInput.OnCellFocus.InvokeAsync(cellInput.Cell));
        await sut.InvokeAsync(() => buttonGroup.OnValueChanged.InvokeAsync(new CellValueChangedEventArgs(1)));

        // Assert
        var victoryOverlay = sut.Find(".victory-overlay");
        victoryOverlay.Should().NotBeNull();
    }

    [Fact]
    public async Task Game_WhenPuzzleSolved_DeletesGame()
    {
        // Arrange
        var gameState = new GameStateMemory(PuzzleId, PuzzleFactory.GetSolvedPuzzle().GetAllCells())
        {
            Alias = Alias
        };
        _mockGameStateManager.SetupLoadGameAsync(gameState);
        var sut = RenderComponent<Game>(x => x.Add(p => p.PuzzleId, PuzzleId));
        var buttonGroup = sut.FindComponent<GameControls>().Instance;
        var cellInput = sut.FindComponent<CellInput>().Instance;

        // Act
        await sut.InvokeAsync(() => cellInput.OnCellFocus.InvokeAsync(cellInput.Cell));
        await sut.InvokeAsync(() => buttonGroup.OnValueChanged.InvokeAsync(new CellValueChangedEventArgs(1)));

        // Assert
        _mockGameStateManager.VerifyDeleteGameAsyncCalled(gameState.Alias, gameState.PuzzleId, Times.Once);
    }

    [Fact]
    public void OnAfterRenderAsync_LoadsGameState()
    {
        // Arrange

        // Act
        RenderComponent<Game>(parameters => parameters.Add(p => p.PuzzleId, PuzzleId));

        // Assert
        _mockGameStateManager.VerifyLoadsAsyncCalled(Alias, PuzzleId, Times.Once);
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
        var gameState = new GameStateMemory(PuzzleId, []);
        _mockGameStateManager.Setup(x => x.UndoGameAsync(Alias, PuzzleId)).ReturnsAsync(gameState);
        var game = RenderComponent<Game>(parameters => parameters.Add(p => p.PuzzleId, PuzzleId));

        // Act
        await game.InvokeAsync(() => game.Instance.HandleUndo());

        // Assert
        _mockGameStateManager.VerifyUndoAsyncCalled(Alias, PuzzleId, Times.Once);
    }

    [Fact]
    public async Task HandleReset_ShouldLoadInitialGameStateAndUpdatePuzzle()
    {
        // Arrange
        var gameState = new GameStateMemory(PuzzleId, []);
        _mockGameStateManager.Setup(x => x.ResetGameAsync(Alias, PuzzleId)).ReturnsAsync(gameState);
        var game = RenderComponent<Game>(parameters => parameters.Add(p => p.PuzzleId, PuzzleId));

        // Act
        await game.InvokeAsync(() => game.Instance.HandleReset());

        // Assert
        _mockGameStateManager.VerifyResetAsyncCalled(Alias, PuzzleId, Times.Once);
    }

    [Fact]
    public async Task Game_OnLocationChanging_PausesSession()
    {
        // Arrange
        var sut = RenderComponent<Game>();
        var navigationManager = Services.GetRequiredService<NavigationManager>();

        // Act
        await sut.InvokeAsync(() => navigationManager.NavigateTo("/another-page"));

        // Assert
        _mockGameSessionManager.Verify(x => x.PauseSession(), Times.Once);
    }

    [Fact]
    public void Game_VictoryDisplay_ShowsWhenPuzzleIsSolved()
    {
        // Arrange
        var gameState = new GameStateMemory(PuzzleId, PuzzleFactory.GetSolvedPuzzle().GetAllCells())
        {
            Alias = Alias
        };
        _mockGameStateManager.SetupLoadGameAsync(gameState);
        var sut = RenderComponent<Game>();

        // Act
        var victoryDisplay = sut.FindComponent<VictoryDisplay>();

        // Assert
        victoryDisplay.Instance.IsVictory.Should().BeTrue();
    }

    [Fact]
    public async Task Game_GameBoard_OnCellFocus_UpdatesSelectedCell()
    {
        // Arrange
        var sut = RenderComponent<Game>();
        var gameBoard = sut.FindComponent<GameBoard>();

        // Act
        await sut.InvokeAsync(() => gameBoard.Instance.OnCellFocus.InvokeAsync(new Cell(1, 1)));

        // Assert
        sut.Instance.SelectedCell.Should().BeEquivalentTo(new Cell(1, 1));
    }

    [Fact]
    public async Task Game_WhenCellChanged_RecordsMove()
    {
        // Arrange
        var sut = RenderComponent<Game>();
        var buttonGroup = sut.FindComponent<GameControls>().Instance;
        var cellInput = sut.FindComponent<CellInput>().Instance;

        // Act
        await sut.InvokeAsync(() => cellInput.OnCellFocus.InvokeAsync(cellInput.Cell));
        await sut.InvokeAsync(() => buttonGroup.OnValueChanged.InvokeAsync(new CellValueChangedEventArgs(3)));

        // Assert
        _mockGameSessionManager.VerifyMoveRecorded(Times.Once);
    }
}