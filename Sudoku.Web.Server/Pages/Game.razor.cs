using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Sudoku.Web.Server.EventArgs;
using Sudoku.Web.Server.Services;

namespace Sudoku.Web.Server.Pages;

public partial class Game
{
    private Cell _selectedCell = new(0, 0);
    private IDisposable? _locationChangingRegistration;

    [Parameter] public string? PuzzleId { get; set; }
    [Inject] public IInvalidCellNotificationService? InvalidCellNotificationService { get; set; }
    [Inject] public IGameNotificationService? GameNotificationService { get; set; }
    [Inject] private ICellFocusedNotificationService? CellFocusedNotificationService { get; set; }
    [Inject] private IGameStateManager? GameStateManager { get; set; }
    [Inject] private IGameSessionManager SessionManager { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;

    public ISudokuPuzzle Puzzle { get; set; } = new SudokuPuzzle();

    protected override async Task OnInitializedAsync()
    {
        _locationChangingRegistration = NavigationManager.RegisterLocationChangingHandler(OnLocationChanging);
        var gameState = await GameStateManager!.LoadGameAsync(PuzzleId!);
        await SessionManager.StartNewSession(gameState!);
        Puzzle.Load(gameState);

        GameNotificationService!.NotifyGameStarted();
    }

    private async ValueTask OnLocationChanging(LocationChangingContext context)
    {
        await SessionManager.PauseSession();
        _locationChangingRegistration!.Dispose();
    }

    private void HandleSetSelectedCell(Cell cell)
    {
        _selectedCell = cell;
    }

    public async Task HandleUndo()
    {
        await SessionManager.PauseSession();
        var gameState = await GameStateManager!.UndoAsync(PuzzleId!);
        await SessionManager.StartNewSession(gameState!);
        Puzzle.Load(gameState);
        StateHasChanged();
    }

    private Task HandleCellChanged(CellChangedEventArgs args)
    {
        return HandleCellUpdate(args.Row, args.Column, args.Value);
    }

    private Task HandleCellValueChanged(CellValueChangedEventArgs args)
    {
        return HandleCellUpdate(_selectedCell.Row, _selectedCell.Column, args.Value);
    }

    private async Task HandleCellUpdate(int row, int column, int? value)
    {
        Puzzle.SetCell(row, column, value);
        var isValid = Puzzle.IsValid();
        InvalidCellNotificationService!.Notify(Puzzle.Validate().ToList());
        SessionManager.CurrentSession.RecordMove(isValid);

        if (Puzzle.IsSolved())
        {
            await SessionManager.EndSession();
            await GameStateManager!.DeleteGameAsync(PuzzleId!);
            GameNotificationService!.NotifyGameEnded();
        }

        StateHasChanged();
    }
}
