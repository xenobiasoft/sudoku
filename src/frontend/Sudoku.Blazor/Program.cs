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
var appInsightsConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];

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

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHealthChecks("/health-check");

app.Run();
