using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Sudoku.Web.Server.Pages
{
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	[IgnoreAntiforgeryToken]
	public class ErrorModel(ILogger<ErrorModel> logger, IWebHostEnvironment environment) : PageModel
    {
		public string? RequestId { get; set; }
		public int StatusCode { get; set; }
		public string? StatusMessage { get; set; }
		public string? ErrorMessage { get; set; }
		public string? ExceptionDetails { get; set; }
		public string? RequestPath { get; set; }

		public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
		public bool ShowExceptionDetails { get; set; }

        public void OnGet()
		{
			RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
			StatusCode = HttpContext.Response.StatusCode;
			RequestPath = HttpContext.Request.Path;
			ShowExceptionDetails = environment.IsDevelopment();

			// Get status code and original path from exception handler
			var statusCodeFeature = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IStatusCodeReExecuteFeature>();
			if (statusCodeFeature != null)
			{
				RequestPath = statusCodeFeature.OriginalPath;
			}

			// Get exception details if available
			var exceptionFeature = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
			if (exceptionFeature != null)
			{
				var exception = exceptionFeature.Error;
				ErrorMessage = exception.Message;
				
				if (environment.IsDevelopment())
				{
					ExceptionDetails = exception.ToString();
				}

				// Log the exception with context
				logger.LogError(
					exception,
					"Unhandled exception occurred. Path: {Path}, RequestId: {RequestId}, StatusCode: {StatusCode}",
					RequestPath ?? exceptionFeature.Path,
					RequestId,
					StatusCode
				);
			}
			else
			{
				// Log status code errors (like 404)
				logger.LogWarning(
					"Error response. Path: {Path}, RequestId: {RequestId}, StatusCode: {StatusCode}",
					RequestPath,
					RequestId,
					StatusCode
				);
			}

			// Set user-friendly status message
			StatusMessage = StatusCode switch
			{
				400 => "Bad Request",
				401 => "Unauthorized",
				403 => "Forbidden",
				404 => "Page Not Found",
				500 => "Internal Server Error",
				502 => "Bad Gateway",
				503 => "Service Unavailable",
				_ => "An Error Occurred"
			};
		}
	}
}