using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.EventArgs;
using Sudoku.Web.Server.Services;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Pages;

public partial class Game
{
    private Cell _selectedCell = new(0, 0);
    private GameTimer _gameTimer = new();
    private GameStateMemory? _currentGameState;
    
    [Parameter] public string? PuzzleId { get; set; }
    [Inject] public ISudokuGame? SudokuGame { get; set; }
    [Inject] public IInvalidCellNotificationService? InvalidCellNotificationService { get; set; }
    [Inject] public IGameNotificationService? GameNotificationService { get; set; }
    [Inject] private ICellFocusedNotificationService? CellFocusedNotificationService { get; set; }
    [Inject] private IGameStateManager? GameStateManager { get; set; }

    public ISudokuPuzzle Puzzle { get; set; } = new SudokuPuzzle();

    protected override async Task OnInitializedAsync()
    {
        _currentGameState = await SudokuGame!.LoadAsync(PuzzleId!);

        Puzzle.Load(PuzzleId, _currentGameState.Board);

        GameNotificationService!.NotifyGameStarted();
    }

    private void HandleSetSelectedCell(Cell cell)
    {
        _selectedCell = cell;
    }

    public async Task HandleUndo()
    {
        _currentGameState = await GameStateManager!.UndoAsync(PuzzleId!);
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

    private Task HandleCellUpdate(int row, int column, int? value)
    {
        Puzzle.SetCell(row, column, value);
        InvalidCellNotificationService!.Notify(Puzzle.Validate().ToList());

        if (Puzzle.IsSolved())
        {
            GameNotificationService!.NotifyGameEnded();
        }

        _currentGameState = new GameStateMemory(PuzzleId, Puzzle.GetAllCells())
        {
            InvalidMoves = 0,
            TotalMoves = _currentGameState!.TotalMoves + 1,
            PlayDuration = _gameTimer.ElapsedGameTime,
        };

        return GameStateManager!.SaveGameAsync(_currentGameState);
    }
}
