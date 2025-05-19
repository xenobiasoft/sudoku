using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Sudoku.Web.Server.EventArgs;
using Sudoku.Web.Server.Services;

namespace Sudoku.Web.Server.Pages;

public partial class Game
{
    private IDisposable? _locationChangingRegistration;

    [Parameter] public string? PuzzleId { get; set; }
    [Inject] public IInvalidCellNotificationService? InvalidCellNotificationService { get; set; }
    [Inject] public IGameNotificationService? GameNotificationService { get; set; }
    [Inject] private ICellFocusedNotificationService? CellFocusedNotificationService { get; set; }
    [Inject] private IGameStateManager? GameStateManager { get; set; }
    [Inject] private IGameSessionManager SessionManager { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;

    public ISudokuPuzzle Puzzle { get; set; } = new SudokuPuzzle();
    public Cell SelectedCell { get; private set; } = new(0, 0);
    private bool IsPencilMode { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        _locationChangingRegistration = NavigationManager.RegisterLocationChangingHandler(OnLocationChanging);
        var gameState = await GameStateManager!.LoadGameAsync(PuzzleId!);
        await SessionManager.StartNewSession(gameState!);
        Puzzle.Load(gameState);

        GameNotificationService!.NotifyGameStarted();
        StateHasChanged();
    }

    private async ValueTask OnLocationChanging(LocationChangingContext context)
    {
        await SessionManager.PauseSession();
        _locationChangingRegistration!.Dispose();
    }

    private void HandleSetSelectedCell(Cell cell)
    {
        SelectedCell = cell;
    }

    public async Task HandleReset()
    {
        await SessionManager.PauseSession();
        var gameState = await GameStateManager!.ResetGameAsync(PuzzleId!);
        SessionManager.ResumeSession(gameState);
        Puzzle.Load(gameState);
        StateHasChanged();
    }

    public async Task HandleUndo()
    {
        await SessionManager.PauseSession();
        var gameState = await GameStateManager!.UndoGameAsync(PuzzleId!);
        SessionManager.ResumeSession(gameState);
        Puzzle.Load(gameState);
        StateHasChanged();
    }

    private Task HandleCellChanged(CellChangedEventArgs args)
    {
        return HandleCellUpdate(args.Row, args.Column, args.Value);
    }

    private Task HandleCellValueChanged(CellValueChangedEventArgs args)
    {
        return HandleCellUpdate(SelectedCell.Row, SelectedCell.Column, args.Value);
    }

    private async Task HandleCellUpdate(int row, int column, int value)
    {
        Puzzle.SetCell(row, column, value);
        var isValid = Puzzle.IsValid();
        InvalidCellNotificationService!.Notify(Puzzle.Validate().ToList());
        await SessionManager.RecordMove(isValid);

        if (Puzzle.IsSolved())
        {
            await SessionManager.EndSession();
            await GameStateManager!.DeleteGameAsync(PuzzleId!);
            GameNotificationService!.NotifyGameEnded();
        }

        StateHasChanged();
    }

    private void HandlePencilModeToggle(bool isPencilMode)
    {
        IsPencilMode = isPencilMode;
        StateHasChanged();
    }

    private void HandlePossibleValueChanged(CellPossibleValueChangedEventArgs arg)
    {
        var possibleValues = SelectedCell.PossibleValues;

        if (possibleValues.Contains(arg.Value))
        {
            possibleValues.Remove(arg.Value);
        }
        else
        {
            possibleValues.Add(arg.Value);
        }

        SelectedCell.PossibleValues = possibleValues;
        StateHasChanged();
    }
}
