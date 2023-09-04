using Microsoft.AspNetCore.Components;

namespace Sudoku.Web.Pages;

public partial class Index
{
	[Inject] private NavigationManager NavigationManager { get; set; } = default!;

	public void LoadNewGame()
	{
		NavigationManager.NavigateTo("/game");
	}
}