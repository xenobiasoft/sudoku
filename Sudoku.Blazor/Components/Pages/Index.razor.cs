using Microsoft.AspNetCore.Components;

namespace Sudoku.Blazor.Components.Pages;

public partial class Index
{
    [Inject] public required NavigationManager NavigationManager { get; set; }

    private void Start()
    {
        NavigationManager.NavigateTo("/start");
    }
}