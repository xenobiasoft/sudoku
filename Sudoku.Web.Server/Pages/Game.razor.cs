using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.EventArgs;
using Sudoku.Web.Server.Services;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Pages;

public partial class Game
{
    private IDisposable? _locationChangingRegistration;
    private Cell _selectedCell = new(0, 0);
    private GameStateMemory? _currentGameState = new();
    
    [Parameter] public string? PuzzleId { get; set; }
    [Inject] public IInvalidCellNotificationService? InvalidCellNotificationService { get; set; }
    [Inject] public IGameNotificationService? GameNotificationService { get; set; }
    [Inject] private ICellFocusedNotificationService? CellFocusedNotificationService { get; set; }
    [Inject] private IGameStateManager? GameStateManager { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    public ISudokuPuzzle Puzzle { get; set; } = new SudokuPuzzle();

    protected override async Task OnInitializedAsync()
    {
        _locationChangingRegistration = NavigationManager.RegisterLocationChangingHandler(OnLocationChanging);
        _currentGameState = UndoPreservingGamePlay(await GameStateManager!.LoadGameAsync(PuzzleId!));
        _currentGameState!.Resume();
        Puzzle.Load(PuzzleId, _currentGameState!.Board);

        GameNotificationService!.NotifyGameStarted();
    }
    
    private async ValueTask OnLocationChanging(LocationChangingContext context)
    {
        await GameStateManager!.SaveGameAsync(_currentGameState!);
    }

    private void HandleSetSelectedCell(Cell cell)
    {
        _selectedCell = cell;
    }

    public async Task HandleUndo()
    {
        _currentGameState!.Pause();
        _currentGameState = await GameStateManager!.UndoAsync(PuzzleId!);
        _currentGameState!.Resume();
        Puzzle.Load(_currentGameState.PuzzleId, _currentGameState.Board);
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
        InvalidCellNotificationService!.Notify(Puzzle.Validate().ToList());

        if (Puzzle.IsSolved())
        {
            await GameStateManager!.DeleteGameAsync(PuzzleId!);
            GameNotificationService!.NotifyGameEnded();
        }

        await SaveGameStateAsync();
    }

    private GameStateMemory? UndoPreservingGamePlay(GameStateMemory? undoState)
    {
        undoState!.PlayDuration = _currentGameState!.PlayDuration;
        undoState.LastResumeTime = _currentGameState.LastResumeTime;

        return undoState;
    }

    private async Task SaveGameStateAsync()
    {
        _currentGameState = new GameStateMemory(PuzzleId, Puzzle.GetAllCells())
        {
            InvalidMoves = _currentGameState!.InvalidMoves + (Puzzle.IsValid() ? 0 : 1),
            TotalMoves = _currentGameState!.TotalMoves + 1,
            PlayDuration = _currentGameState.GetTotalPlayDuration()
        };

        await GameStateManager!.SaveGameAsync(_currentGameState);
        StateHasChanged();
    }
}
