using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace Sudoku.Web.Server.Pages;

public partial class Error
{
    [Inject] public required ILogger<Error> Logger { get; set; }
    [Inject] public required IWebHostEnvironment Environment { get; set; }

    [CascadingParameter]
    public HttpContext? HttpContext { get; set; }

    public string? RequestId { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ExceptionDetails { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public bool ShowExceptionDetails => Environment.IsDevelopment();

    protected override void OnInitialized()
    {
        RequestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier;

        Logger.LogInformation("Error page loaded. RequestId: {RequestId}", RequestId);
    }

    public void SetException(Exception exception)
    {
        ErrorMessage = exception.Message;
        
        if (Environment.IsDevelopment())
        {
            ExceptionDetails = exception.ToString();
        }

        Logger.LogError(exception, "Error displayed to user. RequestId: {RequestId}", RequestId);
    }
}
