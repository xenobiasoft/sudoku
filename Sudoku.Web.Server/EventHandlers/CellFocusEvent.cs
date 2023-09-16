using Microsoft.AspNetCore.Components;

namespace Sudoku.Web.Server.EventHandlers
{
    [EventHandler("oncellfocus", typeof(CellFocusEventArgs), enableStopPropagation: true, enablePreventDefault: true)]
    public static class CellFocusEvent
    {
    }
}
