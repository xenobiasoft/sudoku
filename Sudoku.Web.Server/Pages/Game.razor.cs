using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Sudoku.Web.Server.EventArgs;
using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.Abstractions.V2;

namespace Sudoku.Web.Server.Pages;

public partial class Game
{
    private IDisposable? _locationChangingRegistration;

    [Parameter] public string? PuzzleId { get; set; }
    [Inject] public required INotificationService NotificationService { get; set; }
    [Inject] public required IGameManager GameManager { get; set; }
    [Inject] public required IPlayerManager PlayerManager { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }

    private GameModel CurrentGame => GameManager.Game;
    public CellModel SelectedCell { get; private set; } = new();
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
        Alias = await PlayerManager.GetCurrentPlayerAsync();
        await GameManager.LoadGameAsync(Alias!, PuzzleId!);

        await GameManager.StartGameAsync();
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

    private void HandleSetSelectedCell(CellModel cell)
    {
        SelectedCell = cell;
        StateHasChanged();
    }

    public async Task HandleReset()
    {
        await UpdateGameStateAsync(() => GameManager.ResetGameAsync());
    }

    public async Task HandleUndo()
    {
        await UpdateGameStateAsync(() => GameManager.UndoGameAsync());
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
        NotificationService.NotifyInvalidCells(invalidCells);
    }

    private async Task ValidateAndUpdateGameState(int row, int column, int? value)
    {
        var isValid = CurrentGame.IsValid();
        await GameManager.RecordMove(row, column, value, isValid);

        if (CurrentGame.IsSolved())
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

    // Now async: persist possible value changes through GameManager
    private async Task HandlePossibleValueChanged(CellPossibleValueChangedEventArgs arg)
    {
        // Ensure we have a selected cell
        if (SelectedCell == null) return;

        var row = SelectedCell.Row;
        var column = SelectedCell.Column;

        if (!arg.Value.HasValue)
        {
            // Clear all possible values
            SelectedCell.PossibleValues.Clear();
            try
            {
                await GameManager.ClearPossibleValuesAsync(row, column);
            }
            catch
            {
                // ignore persistence failures for now
            }
        }
        else
        {
            var val = arg.Value.Value;
            var containsBefore = SelectedCell.PossibleValues.Contains(val);

            if (containsBefore)
            {
                // remove
                SelectedCell.PossibleValues.Remove(val);
                try
                {
                    await GameManager.RemovePossibleValueAsync(row, column, val);
                }
                catch
                {
                    // ignore persistence failures for now
                }
            }
            else
            {
                // add
                SelectedCell.PossibleValues.Add(val);
                try
                {
                    await GameManager.AddPossibleValueAsync(row, column, val);
                }
                catch
                {
                    // ignore persistence failures for now
                }
            }
        }

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
