using Microsoft.AspNetCore.Components;

namespace Sudoku.Web.Server.Pages;

public partial class Index
{
	[Inject] private NavigationManager NavigationManager { get; set; } = default!;

	private void LoadNewGame()
	{
		NavigationManager.NavigateTo("/game");
	}
}