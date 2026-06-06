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

    var cosmosDb = builder.AddAzureCosmosDB("CosmosDb").RunAsEmulator();
    var storage = builder.AddAzureStorage("storage").RunAsEmulator();
    var puzzleBlobs = storage.AddBlobs("blobs");

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
        .WithReference(puzzleBlobs)
        .WithExternalHttpEndpoints()
        .WaitFor(cosmosDb)
        .WaitFor(storage);

    logger.LogInformation("Configuring Sudoku Functions project...");
    builder.AddAzureFunctionsProject<Projects.Sudoku_Functions>("sudoku-functions")
        .WithHostStorage(storage)
        .WithReference(puzzleBlobs)
        .WithReference(cosmosDb)
        .WaitFor(cosmosDb)
        .WaitFor(storage);

    logger.LogInformation("Configuring Sudoku React project...");
    builder.AddNpmApp("sudoku-react", "../../frontend/Sudoku.React", "dev")
        .WithReference(api)
        .WithEnvironment("VITE_API_BASE_URL", api.GetEndpoint("https"))
        .WithHttpEndpoint(port: 5173, env: "VITE_PORT")
        .WithExternalHttpEndpoints()
        .WaitFor(api)
        .PublishAsDockerFile();

    logger.LogInformation("Building and starting application...");
    var app = builder.Build();

    app.Run();
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Failed to start Sudoku Distributed Application");
    throw;
}
