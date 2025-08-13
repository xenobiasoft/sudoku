var builder = DistributedApplication.CreateBuilder(args);

var appConfig = builder.AddConnectionString("appconfig");
var cosmosDb = builder.AddConnectionString("CosmosDb");

builder.AddProject<Projects.Sudoku_Api>("sudoku-api")
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
    .WithExternalHttpEndpoints()
    .WaitFor(cosmosDb);

builder.Build().Run();
