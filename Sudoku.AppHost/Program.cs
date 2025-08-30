using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = DistributedApplication.CreateBuilder(args);

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

using var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});
var logger = loggerFactory.CreateLogger<Program>();

try
{
    logger.LogInformation("Starting Sudoku Distributed Application...");

    logger.LogInformation("Configuring connection strings...");
    var appConfig = builder.AddConnectionString("appconfig");
    var cosmosDb = builder.AddConnectionString("CosmosDb");
    logger.LogInformation("Connection strings configured successfully");

    logger.LogInformation("Configuring Sudoku API project...");
    builder.AddProject<Projects.Sudoku_Api>("sudoku-api")
        .WithUrlForEndpoint("https", url =>
        {
            url.DisplayText = "Swagger (HTTPS)";
            url.Url = "/swagger";
            logger.LogInformation("Configured HTTPS Swagger endpoint for Sudoku API");
        })
        .WithUrlForEndpoint("http", url =>
        {
            url.DisplayText = "Swagger (HTTP)";
            url.Url = "/swagger";
            logger.LogInformation("Configured HTTP Swagger endpoint for Sudoku API");
        })
        .WithReference(cosmosDb)
        .WithReference(appConfig)
        .WaitFor(cosmosDb);
    logger.LogInformation("Sudoku API project configured successfully");

    logger.LogInformation("Configuring Sudoku Blazor Server project...");
    builder.AddProject<Projects.Sudoku_Web_Server>("sudoku-blazor")
        .WithReference(cosmosDb)
        .WithReference(appConfig)
        .WithExternalHttpEndpoints()
        .WaitFor(cosmosDb);
    logger.LogInformation("Sudoku Blazor Server project configured successfully");

    logger.LogInformation("Building and starting application...");
    var app = builder.Build();
    
    logger.LogInformation("Sudoku Distributed Application started successfully");
    app.Run();
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Failed to start Sudoku Distributed Application");
    throw;
}
