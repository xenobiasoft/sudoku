using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Sudoku.Web.Server.EventArgs;
using Sudoku.Web.Server.Services.Abstractions;

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
    [Inject] public required IApiBasedGameStateManager ApiGameStateManager { get; set; }
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
        try
        {
            Alias = await AliasService.GetAliasAsync();
            var gameState = await GameStateManager.LoadGameAsync(Alias, PuzzleId!);
            
            if (gameState != null)
            {
                await SessionManager.StartNewSession(gameState);
                Puzzle.Load(gameState);
            }
            else
            {
                // Game not found, navigate to home
                NavigationManager.NavigateTo("/");
            }
        }
        catch (Exception ex)
        {
            // Log error and navigate to home
            Console.WriteLine($"Error loading game: {ex.Message}");
            NavigationManager.NavigateTo("/");
        }
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
        try
        {
            await SessionManager.PauseSession();
            var result = await ApiGameStateManager.ResetGameAsync(Alias, PuzzleId!);
            
            if (result.IsSuccess)
            {
                // Reload the game state after reset
                var gameState = await GameStateManager.LoadGameAsync(Alias, PuzzleId!);
                if (gameState != null)
                {
                    await SessionManager.ResumeSession(gameState);
                    Puzzle.Load(gameState);
                    StateHasChanged();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error resetting game: {ex.Message}");
        }
    }

    public async Task HandleUndo()
    {
        try
        {
            await SessionManager.PauseSession();
            var result = await ApiGameStateManager.UndoGameAsync(Alias, PuzzleId!);
            
            if (result.IsSuccess)
            {
                // Reload the game state after undo
                var gameState = await GameStateManager.LoadGameAsync(Alias, PuzzleId!);
                if (gameState != null)
                {
                    await SessionManager.ResumeSession(gameState);
                    Puzzle.Load(gameState);
                    StateHasChanged();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error undoing move: {ex.Message}");
        }
    }

    private async Task HandleCellChanged(CellChangedEventArgs args) =>
        await HandleCellUpdate(args.Row, args.Column, args.Value);

    private async Task HandleCellValueChanged(CellValueChangedEventArgs args) =>
        await HandleCellUpdate(SelectedCell.Row, SelectedCell.Column, args.Value);

    private async Task HandleCellUpdate(int row, int column, int? value)
    {
        try
        {
            // Make the move via API
            var result = await ApiGameStateManager.MakeMoveAsync(Alias, PuzzleId!, row, column, value);
            
            if (result.IsSuccess)
            {
                // Update local puzzle state
                UpdatePuzzleCell(row, column, value);
                await ValidateAndUpdateGameState();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error making move: {ex.Message}");
        }
    }

    private void UpdatePuzzleCell(int row, int column, int? value)
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

    private async Task HandlePossibleValueChanged(CellPossibleValueChangedEventArgs arg)
    {
        try
        {
            if (!arg.Value.HasValue)
            {
                // Clear possible values
                var result = await ApiGameStateManager.ClearPossibleValuesAsync(Alias, PuzzleId!, SelectedCell.Row, SelectedCell.Column);
                if (result.IsSuccess)
                {
                    UpdatePossibleValues(arg.Value);
                }
            }
            else
            {
                // Check if we need to add or remove the possible value
                if (SelectedCell.PossibleValues.Contains(arg.Value.Value))
                {
                    // Remove possible value
                    var result = await ApiGameStateManager.RemovePossibleValueAsync(Alias, PuzzleId!, SelectedCell.Row, SelectedCell.Column, arg.Value.Value);
                    if (result.IsSuccess)
                    {
                        UpdatePossibleValues(arg.Value);
                    }
                }
                else
                {
                    // Add possible value
                    var result = await ApiGameStateManager.AddPossibleValueAsync(Alias, PuzzleId!, SelectedCell.Row, SelectedCell.Column, arg.Value.Value);
                    if (result.IsSuccess)
                    {
                        UpdatePossibleValues(arg.Value);
                    }
                }
            }
            
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating possible values: {ex.Message}");
        }
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
