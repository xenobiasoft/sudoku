var builder = DistributedApplication.CreateBuilder(args);

var keyVault = builder.AddConnectionString("AzureKeyVault");

// Add CosmosDB resource with the basic setup
var cosmosDb = builder.AddAzureCosmosDB("cosmosdb");

builder.AddProject<Projects.Sudoku_Api>("sudoku-api")
    .WithReference(cosmosDb)
    .WithReference(keyVault);

builder.AddProject<Projects.Sudoku_Web_Server>("sudoku-blazor")
    .WithReference(cosmosDb)
    .WithReference(keyVault)
    .WithExternalHttpEndpoints();

builder.Build().Run();
