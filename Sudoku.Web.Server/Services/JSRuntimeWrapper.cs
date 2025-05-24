using Microsoft.JSInterop;
using Sudoku.Web.Server.Services.Abstractions;

namespace Sudoku.Web.Server.Services;

public class JsRuntimeWrapper(IJSRuntime jsRuntime) : IJsRuntimeWrapper
{
    public ValueTask<string> GetAsync(string key)
    {
        return jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
    }

    public ValueTask SetAsync(string key, string value)
    {
        return jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
    }
}