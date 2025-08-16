var builder = DistributedApplication.CreateBuilder(args);

var appConfig = builder.AddConnectionString("appconfig");
var cosmosDb = builder.AddConnectionString("CosmosDb");

var sudokuApi = builder.AddProject<Projects.Sudoku_Api>("sudoku-api")
    .WithUrlForEndpoint("https", url =>
    {
        url.DisplayText = "Swagger (HTTPS)";
        url.Url = "/swagger";
    })
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Swagger (HTTP)";
        url.Url = "/swagger";
    })
    .WithReference(cosmosDb)
    .WithReference(appConfig)
    .WaitFor(cosmosDb);

builder.AddProject<Projects.Sudoku_Web_Server>("sudoku-blazor")
    .WithReference(cosmosDb)
    .WithReference(appConfig)
    .WithReference(sudokuApi) // Add reference to the API project
    .WithExternalHttpEndpoints()
    .WaitFor(cosmosDb)
    .WaitFor(sudokuApi); // Wait for API to be ready

builder.Build().Run();
