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
    [Inject] public required INotificationService NotificationService { get; set; }
    [Inject] public required IGameManager GameManager { get; set; }
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
        var gameState = await GameManager.LoadGameAsync(Alias, PuzzleId!);
        await GameManager.StartNewSession(gameState!);
        Puzzle.Load(gameState);
    }

    private void NotifyGameStart()
    {
        NotificationService.NotifyGameStarted();
        StateHasChanged();
    }

    private async ValueTask OnLocationChanging(LocationChangingContext context)
    {
        await GameManager.PauseSession();
        _locationChangingRegistration?.Dispose();
    }

    private void HandleSetSelectedCell(Cell cell)
    {
        SelectedCell = cell;
        StateHasChanged();
    }

    public async Task HandleReset()
    {
        await UpdateGameStateAsync(() => GameManager.ResetGameAsync(Alias, PuzzleId!));
    }

    public async Task HandleUndo()
    {
        await UpdateGameStateAsync(() => GameManager.UndoGameAsync(Alias, PuzzleId!));
    }

    private async Task UpdateGameStateAsync(Func<Task<GameStateMemory>> stateUpdateAction)
    {
        await GameManager.PauseSession();
        var gameState = await stateUpdateAction();
        GameManager.ResumeSession(gameState);
        Puzzle.Load(gameState);
        StateHasChanged();
    }

    private Task HandleCellChanged(CellChangedEventArgs args) =>
        HandleCellUpdate(args.Row, args.Column, args.Value);

    private Task HandleCellValueChanged(CellValueChangedEventArgs args) =>
        HandleCellUpdate(SelectedCell.Row, SelectedCell.Column, args.Value);

    private async Task HandleCellUpdate(int row, int column, int? value)
    {
        UpdatePuzzleCell(row, column, value);
        await ValidateAndUpdateGameState();
    }

    private void UpdatePuzzleCell(int row, int column, int? value)
    {
        Puzzle.SetCell(row, column, value);
        var isValid = Puzzle.IsValid();
        NotificationService.NotifyInvalidCells(Puzzle.Validate().ToList());
    }

    private async Task ValidateAndUpdateGameState()
    {
        await GameManager.RecordMove(Puzzle.IsValid());

        if (Puzzle.IsSolved())
        {
            await HandleGameCompletion();
        }

        StateHasChanged();
    }

    private async Task HandleGameCompletion()
    {
        await GameManager.EndSession();
        await GameManager.DeleteGameAsync(Alias, PuzzleId!);
        NotificationService.NotifyGameEnded();
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

    private void UpdatePossibleValues(int? value)
    {
        var possibleValues = SelectedCell.PossibleValues;
        if (!value.HasValue)
        {
            possibleValues.Clear();
        }
        else if (possibleValues.Contains(value.Value))
        {
            possibleValues.Remove(value.Value);
        }
        else
        {
            possibleValues.Add(value.Value);
        }
        SelectedCell.PossibleValues = possibleValues;
    }
}
