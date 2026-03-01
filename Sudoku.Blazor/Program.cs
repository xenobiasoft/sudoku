using Azure.Identity;
using BlazorApplicationInsights;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Sudoku.Blazor;
using Sudoku.Blazor.Components;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder
    .Logging
    .AddConsole()
    .AddDebug()
    .AddAzureWebAppDiagnostics();

builder.Configuration.AddEnvironmentVariables();


// Try to get the key vault URI from the connection string (Aspire way) or direct configuration
var vaultUri = builder.Configuration["ConnectionStrings:AzureKeyVault"]
               ?? builder.Configuration["KeyVault:VaultUri"]
               ?? string.Empty;

if (!string.IsNullOrEmpty(vaultUri))
{
    builder.Configuration.AddAzureKeyVault(new Uri(vaultUri), new DefaultAzureCredential());
}

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHealthChecks();

// Get Application Insights connection string
var appInsightsConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]
                                  ?? builder.Configuration["AppInsightsConnectionString"];

// Register Blazor game services (includes HttpClient configuration for API clients)
builder.Services
    .RegisterBlazorGameServices(builder.Configuration);

// Only configure Application Insights if connection string is provided
if (!string.IsNullOrEmpty(appInsightsConnectionString))
{
    builder.Services
        .AddBlazorApplicationInsights(x =>
        {
            x.ConnectionString = appInsightsConnectionString;
        })
        .AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions
        {
            ConnectionString = appInsightsConnectionString,
            EnableQuickPulseMetricStream = true
        });

    // Configure Application Insights logging
    builder.Logging
        .AddApplicationInsights(
            configureTelemetryConfiguration: (config) => config.ConnectionString = appInsightsConnectionString,
            configureApplicationInsightsLoggerOptions: (options) => { });
}

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

// Make sure the browser always re-checks the service worker + manifest.
// Critical for reliable updates and avoiding stale SW behavior on Linux App Service.
app.Use(async (context, next) =>
{
    var path = context.Request.Path;

    if (path.Equals("/service-worker.js") ||
        path.Equals("/manifest.webmanifest") ||
        path.Equals("/manifest.json") ||
        path.Equals("/offline.html"))
    {
        context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
        context.Response.Headers.Pragma = "no-cache";
        context.Response.Headers.Expires = "0";
    }

    await next();
});

// For navigations (HTML), discourage caching. Helps prevent stale host pages.
app.Use(async (context, next) =>
{
    await next();

    // Only touch successful HTML navigations
    if (context.Response.StatusCode == 200 &&
        context.Request.Method == "GET" &&
        context.Request.Headers.Accept.ToString().Contains("text/html", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
        context.Response.Headers.Pragma = "no-cache";
        context.Response.Headers.Expires = "0";
    }
});

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHealthChecks("/health-check");

app.Run();
