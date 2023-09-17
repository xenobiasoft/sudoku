using Microsoft.AspNetCore.Components;

namespace Sudoku.Web.Server.Components
{
    public partial class ButtonGroup
    {
        [Parameter]
        public EventCallback<int> NumberClicked { get; set; }
        
        private async Task SetValue(int value)
        {
            await NumberClicked.InvokeAsync(value);
        }
    }
}
