using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Sudoku.Web.Server.EventArgs;
using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.Abstractions;

namespace Sudoku.Web.Server.Pages;

public partial class Game
{
    private IDisposable? _locationChangingRegistration;

    [Parameter] public string? PuzzleId { get; set; }
    [Inject] public required INotificationService NotificationService { get; set; }
    [Inject] public required IGameManager GameManager { get; set; }
    [Inject] public required IPlayerManager PlayerManager { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required ILogger<Game> Logger { get; set; }

    private GameModel CurrentGame => GameManager.Game;
    public CellModel SelectedCell { get; private set; } = new();
    private bool IsPencilMode { get; set; }
    private string Alias { get; set; } = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        Logger.LogInformation("Game page first render completed for puzzle {PuzzleId}", PuzzleId);
        await InitializeGameAsync();
    }

    private async Task InitializeGameAsync()
    {
        try
        {
            Logger.LogInformation("Initializing game for puzzle {PuzzleId}", PuzzleId);
            RegisterNavigationHandler();
            await LoadGameStateAsync();
            NotifyGameStart();
            Logger.LogInformation("Game initialized successfully for puzzle {PuzzleId}", PuzzleId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to initialize game for puzzle {PuzzleId}", PuzzleId);
            NavigationManager.NavigateTo("/error/500");
        }
    }

    private void RegisterNavigationHandler()
    {
        Logger.LogDebug("Registering navigation handler for game {PuzzleId}", PuzzleId);
        _locationChangingRegistration = NavigationManager.RegisterLocationChangingHandler(OnLocationChanging);
    }

    private async Task LoadGameStateAsync()
    {
        Alias = await PlayerManager.GetCurrentPlayerAsync();
        Logger.LogInformation("Loading game {PuzzleId} for player {Alias}", PuzzleId, Alias);
        
        await GameManager.LoadGameAsync(Alias!, PuzzleId!);
        await GameManager.StartGameAsync();
        
        Logger.LogInformation("Game state loaded and started for puzzle {PuzzleId}", PuzzleId);
    }

    private void NotifyGameStart()
    {
        Logger.LogDebug("Notifying game started for puzzle {PuzzleId}", PuzzleId);
        NotificationService.NotifyGameStarted();
        StateHasChanged();
    }

    private async ValueTask OnLocationChanging(LocationChangingContext context)
    {
        try
        {
            Logger.LogInformation("Location changing from game {PuzzleId}, pausing session", PuzzleId);
            await GameManager.PauseSession();
            _locationChangingRegistration?.Dispose();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error while pausing game session on navigation from puzzle {PuzzleId}", PuzzleId);
        }
    }

    private void HandleSetSelectedCell(CellModel cell)
    {
        Logger.LogDebug("Selected cell changed to row {Row}, column {Column} in puzzle {PuzzleId}", cell.Row, cell.Column, PuzzleId);
        SelectedCell = cell;
        StateHasChanged();
    }

    public async Task HandleReset()
    {
        Logger.LogInformation("Resetting game {PuzzleId}", PuzzleId);
        await UpdateGameStateAsync(() => GameManager.ResetGameAsync());
        Logger.LogInformation("Game {PuzzleId} reset successfully", PuzzleId);
    }

    public async Task HandleUndo()
    {
        Logger.LogInformation("Undoing last move in game {PuzzleId}", PuzzleId);
        await UpdateGameStateAsync(() => GameManager.UndoGameAsync());
        Logger.LogInformation("Undo completed for game {PuzzleId}", PuzzleId);
    }

    private async Task UpdateGameStateAsync(Func<Task<GameModel>> action)
    {
        await GameManager.PauseSession();
        _ = await action();
        await GameManager.ResumeSession();
        StateHasChanged();
    }

    private Task HandleCellChanged(CellChangedEventArgs args) => HandleCellUpdate(args.Row, args.Column, args.Value);

    private Task HandleCellValueChanged(CellValueChangedEventArgs args) => HandleCellUpdate(SelectedCell.Row, SelectedCell.Column, args.Value);

    private async Task HandleCellUpdate(int row, int column, int? value)
    {
        Logger.LogDebug("Cell update at row {Row}, column {Column} with value {Value} in puzzle {PuzzleId}", row, column, value, PuzzleId);
        UpdateGameCell(row, column, value);
        await ValidateAndUpdateGameState(row, column, value);
    }

    private void UpdateGameCell(int row, int column, int? value)
    {
        var cell = CurrentGame.Cells.FirstOrDefault(c => c.Row == row && c.Column == column);
        if (cell != null)
        {
            cell.Value = value;
            cell.HasValue = value.HasValue;
        }

        var invalidCells = CurrentGame.Validate();
        Logger.LogDebug("Cell validation completed for puzzle {PuzzleId}, invalid cells count: {InvalidCellsCount}", PuzzleId, invalidCells.Count());
        NotificationService.NotifyInvalidCells(invalidCells);
    }

    private async Task ValidateAndUpdateGameState(int row, int column, int? value)
    {
        var isValid = CurrentGame.IsValid();
        Logger.LogDebug("Recording move at row {Row}, column {Column}, valid: {IsValid} for puzzle {PuzzleId}", row, column, isValid, PuzzleId);
        await GameManager.RecordMove(row, column, value, isValid);

        if (CurrentGame.IsSolved())
        {
            Logger.LogInformation("Game {PuzzleId} has been solved by player {Alias}", PuzzleId, Alias);
            await HandleGameCompletion();
        }

        StateHasChanged();
    }

    private async Task HandleGameCompletion()
    {
        Logger.LogInformation("Handling game completion for puzzle {PuzzleId}, player {Alias}", PuzzleId, Alias);
        await GameManager.EndSession();
        await GameManager.DeleteGameAsync(Alias, PuzzleId!);
        NotificationService.NotifyGameEnded();
        Logger.LogInformation("Game completion handled successfully for puzzle {PuzzleId}", PuzzleId);
    }

    private void HandlePencilModeToggle(bool isPencilMode)
    {
        Logger.LogDebug("Pencil mode toggled to {IsPencilMode} for puzzle {PuzzleId}", isPencilMode, PuzzleId);
        IsPencilMode = isPencilMode;
        StateHasChanged();
    }

    private async Task HandlePossibleValueChanged(CellPossibleValueChangedEventArgs arg)
    {
        if (SelectedCell == null) return;

        var row = SelectedCell.Row;
        var column = SelectedCell.Column;

        try
        {
            if (!arg.Value.HasValue)
            {
                Logger.LogDebug("Clearing all possible values for cell at row {Row}, column {Column} in puzzle {PuzzleId}", row, column, PuzzleId);
                SelectedCell.PossibleValues.Clear();
                await GameManager.ClearPossibleValuesAsync(row, column);
            }
            else
            {
                var val = arg.Value.Value;
                var containsBefore = SelectedCell.PossibleValues.Contains(val);

                if (containsBefore)
                {
                    Logger.LogDebug("Removing possible value {Value} from cell at row {Row}, column {Column} in puzzle {PuzzleId}", val, row, column, PuzzleId);
                    SelectedCell.PossibleValues.Remove(val);
                    await GameManager.RemovePossibleValueAsync(row, column, val);
                }
                else
                {
                    Logger.LogDebug("Adding possible value {Value} to cell at row {Row}, column {Column} in puzzle {PuzzleId}", val, row, column, PuzzleId);
                    SelectedCell.PossibleValues.Add(val);
                    await GameManager.AddPossibleValueAsync(row, column, val);
                }
            }

            StateHasChanged();
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to persist possible value change for cell at row {Row}, column {Column} in puzzle {PuzzleId}", row, column, PuzzleId);
        }
    }

    private async Task HandleNavigateToHome()
    {
        Logger.LogInformation("Navigating to home from game {PuzzleId}, pausing and saving game", PuzzleId);
        try
        {
            await GameManager.PauseSession();
            NavigationManager.NavigateTo("/");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error while navigating to home from game {PuzzleId}", PuzzleId);
        }
    }
}
