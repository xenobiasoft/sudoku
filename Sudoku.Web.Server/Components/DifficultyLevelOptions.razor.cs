using Microsoft.AspNetCore.Components;

namespace Sudoku.Web.Server.Components
{
    public partial class DifficultyLevelOptions
    {
        [Parameter]
        public EventCallback<Level> OnDifficultyLevelChanged { get; set; }

        private void OnOptionChanged(ChangeEventArgs e)
        {
            var level = (Level)Enum.Parse(typeof(Level), e.Value.ToString());
            OnDifficultyLevelChanged.InvokeAsync(level);
        }
    }
}
