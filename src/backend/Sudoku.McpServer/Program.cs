using Azure.Identity;
using Azure.Monitor.Query;
using Sudoku.McpServer.Tools;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Azure Monitor Logs client
// Uses DefaultAzureCredential → SystemAssigned Managed Identity in Azure,
// local credentials (az login / VS / environment) in development.
// ---------------------------------------------------------------------------
builder.Services.AddSingleton(_ => new LogsQueryClient(new DefaultAzureCredential()));

// ---------------------------------------------------------------------------
// MCP server — HTTP + SSE transport
// Tools are resolved from DI, so constructor injection works normally.
// ---------------------------------------------------------------------------
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<ApplicationInsightsTools>();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseHttpsRedirection();

// MCP endpoint — SSE on GET /mcp, POST /mcp for messages
app.MapMcp("/mcp");

// Health check for App Service probes
app.MapHealthChecks("/health-check");

app.Run();
