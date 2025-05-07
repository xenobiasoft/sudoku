using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.EventArgs;
using Sudoku.Web.Server.Services;

namespace Sudoku.Web.Server.Pages;

public partial class Game
{
    private Cell _selectedCell = new(0, 0);
    private GameTimer _gameTimer = new();
    
    [Parameter] public string? PuzzleId { get; set; }
    [Inject] public ISudokuGame? SudokuGame { get; set; }
    [Inject] public IInvalidCellNotificationService? InvalidCellNotificationService { get; set; }
    [Inject] public IGameNotificationService? GameNotificationService { get; set; }
    [Inject] private ICellFocusedNotificationService? CellFocusedNotificationService { get; set; }
    [Inject] private IGameStateManager? GameStateManager { get; set; }

    public ISudokuPuzzle Puzzle { get; set; } = new SudokuPuzzle();

    protected override async Task OnInitializedAsync()
    {
        var gameState = await SudokuGame!.LoadAsync(PuzzleId!);

        Puzzle.Load(PuzzleId, gameState.Board);

        GameNotificationService!.NotifyGameStarted();
    }

    private void HandleSetSelectedCell(Cell cell)
    {
        _selectedCell = cell;
    }

    public async Task HandleUndo()
    {
        var gameState = await GameStateManager!.UndoAsync(PuzzleId!);
        Puzzle.Load(gameState.PuzzleId, gameState.Board);
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

    private Task HandleCellUpdate(int row, int column, int? value)
    {
        Puzzle.SetCell(row, column, value);
        InvalidCellNotificationService!.Notify(Puzzle.Validate().ToList());

        if (Puzzle.IsSolved())
        {
            GameNotificationService!.NotifyGameEnded();
        }

        return GameStateManager!.SaveGameAsync(Puzzle.ToGameState());
    }
}
