using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Sudoku.Web.Server.EventArgs;
using Sudoku.Web.Server.Services.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Pages;

public partial class Game
{
    private IDisposable? _locationChangingRegistration;

    [Parameter] public string? PuzzleId { get; set; }
    [Inject] public required IInvalidCellNotificationService InvalidCellNotificationService { get; set; }
    [Inject] public required IGameNotificationService GameNotificationService { get; set; }
    [Inject] public required ICellFocusedNotificationService CellFocusedNotificationService { get; set; }
    [Inject] public required IGameStateManager GameStateManager { get; set; }
    [Inject] public required IGameSessionManager SessionManager { get; set; }
    [Inject] public required IAliasService AliasService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }

    private ISudokuPuzzle Puzzle { get; set; } = new SudokuPuzzle();
    public Cell SelectedCell { get; private set; } = new(0, 0);
    private bool IsPencilMode { get; set; }
    private string Alias { get; set; } = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        await InitializeGameAsync();
    }

    private async Task InitializeGameAsync()
    {
        RegisterNavigationHandler();
        await LoadGameStateAsync();
        NotifyGameStart();
    }

    private void RegisterNavigationHandler()
    {
        _locationChangingRegistration = NavigationManager.RegisterLocationChangingHandler(OnLocationChanging);
    }

    private async Task LoadGameStateAsync()
    {
        Alias = await AliasService.GetAliasAsync();
        var gameState = await GameStateManager.LoadGameAsync(Alias, PuzzleId!);
        await SessionManager.StartNewSession(gameState!);
        Puzzle.Load(gameState);
    }

    private void NotifyGameStart()
    {
        GameNotificationService.NotifyGameStarted();
        StateHasChanged();
    }

    private async ValueTask OnLocationChanging(LocationChangingContext context)
    {
        await SessionManager.PauseSession();
        _locationChangingRegistration?.Dispose();
    }

    private void HandleSetSelectedCell(Cell cell)
    {
        SelectedCell = cell;
        StateHasChanged();
    }

    public async Task HandleReset()
    {
        await UpdateGameStateAsync(() => GameStateManager.ResetGameAsync(Alias, PuzzleId!));
    }

    public async Task HandleUndo()
    {
        await UpdateGameStateAsync(() => GameStateManager.UndoGameAsync(Alias, PuzzleId!));
    }

    private async Task UpdateGameStateAsync(Func<Task<GameStateMemory>> stateUpdateAction)
    {
        await SessionManager.PauseSession();
        var gameState = await stateUpdateAction();
        SessionManager.ResumeSession(gameState);
        Puzzle.Load(gameState);
        StateHasChanged();
    }

    private Task HandleCellChanged(CellChangedEventArgs args) =>
        HandleCellUpdate(args.Row, args.Column, args.Value);

    private Task HandleCellValueChanged(CellValueChangedEventArgs args) =>
        HandleCellUpdate(SelectedCell.Row, SelectedCell.Column, args.Value);

    private async Task HandleCellUpdate(int row, int column, int value)
    {
        UpdatePuzzleCell(row, column, value);
        await ValidateAndUpdateGameState();
    }

    private void UpdatePuzzleCell(int row, int column, int value)
    {
        Puzzle.SetCell(row, column, value);
        var isValid = Puzzle.IsValid();
        InvalidCellNotificationService.Notify(Puzzle.Validate().ToList());
    }

    private async Task ValidateAndUpdateGameState()
    {
        await SessionManager.RecordMove(Puzzle.IsValid());

        if (Puzzle.IsSolved())
        {
            await HandleGameCompletion();
        }

        StateHasChanged();
    }

    private async Task HandleGameCompletion()
    {
        await SessionManager.EndSession();
        await GameStateManager.DeleteGameAsync(Alias, PuzzleId!);
        GameNotificationService.NotifyGameEnded();
    }

    private void HandlePencilModeToggle(bool isPencilMode)
    {
        IsPencilMode = isPencilMode;
        StateHasChanged();
    }

    private void HandlePossibleValueChanged(CellPossibleValueChangedEventArgs arg)
    {
        UpdatePossibleValues(arg.Value);
        StateHasChanged();
    }

    private void UpdatePossibleValues(int value)
    {
        var possibleValues = SelectedCell.PossibleValues;
        if (possibleValues.Contains(value))
        {
            possibleValues.Remove(value);
        }
        else
        {
            possibleValues.Add(value);
        }
        SelectedCell.PossibleValues = possibleValues;
    }
}
