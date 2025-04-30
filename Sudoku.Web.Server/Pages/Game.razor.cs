using Microsoft.AspNetCore.Components;
using Sudoku.Web.Server.Components;
using Sudoku.Web.Server.Services;

namespace Sudoku.Web.Server.Pages;

public partial class Game
{
    private Cell _selectedCell = new(0, 0);
    private GameTimer _gameTimer = new();

    public ISudokuPuzzle Puzzle { get; set; } = new SudokuPuzzle();

    [Parameter] public string? PuzzleId { get; set; }

    [Inject]
	public ISudokuGame? SudokuGame { get; set; }

    [Inject]
    public IInvalidCellNotificationService? InvalidCellNotificationService { get; set; }

    [Inject]
    public IGameNotificationService? GameNotificationService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var gameState = await SudokuGame!.LoadAsync(PuzzleId!);

        Puzzle.Load(gameState.Board);
    }

    private void SetCellValue(int? value)
    {
        _selectedCell.Value = value;

        InvalidCellNotificationService!.Notify(Puzzle.Validate().ToList());

        if (Puzzle.IsSolved())
        {
            GameNotificationService!.NotifyGameEnded();
        }
    }

    private void SetSelectedCell(Cell cell)
    {
        _selectedCell = cell;
    }
}