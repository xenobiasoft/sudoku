using Microsoft.AspNetCore.Components.Web;

namespace Sudoku.Web.Server.Components
{
    public class CellFocusEventArgs : FocusEventArgs
    {
        public int Column { get; set; }
        public int Row { get; set; }
    }
}