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
    
    // Use Azure Cosmos DB Emulator for local development
    // For production, the projects will use connection strings from user secrets or Azure configuration
    var cosmosDb = builder.AddAzureCosmosDB("CosmosDb")
        .RunAsEmulator();

    logger.LogInformation("Configuring Sudoku API project...");
    var api = builder.AddProject<Projects.Sudoku_Api>("sudoku-api")
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
        .WithEnvironment("UseCosmosDb", "true")
        .WaitFor(cosmosDb)
        .WithExternalHttpEndpoints();

    logger.LogInformation("Configuring Sudoku Blazor Server project...");
    builder.AddProject<Projects.Sudoku_Web_Server>("sudoku-blazor")
        .WithReference(cosmosDb)
        .WithReference(api)
        .WithEnvironment("UseCosmosDb", "true")
        .WithExternalHttpEndpoints()
        .WaitFor(cosmosDb);

    logger.LogInformation("Building and starting application...");
    var app = builder.Build();

    app.Run();
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Failed to start Sudoku Distributed Application");
    throw;
}
