using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var keyVault = builder.AddConnectionString("AzureKeyVault");

// Add CosmosDB resource - conditionally use emulator for local development
var cosmosDb = builder.Environment.IsDevelopment() ?
    builder.AddAzureCosmosDB("cosmosdb").RunAsEmulator() : 
    builder.AddAzureCosmosDB("cosmosdb");

builder.AddProject<Projects.Sudoku_Api>("sudoku-api")
    .WithReference(cosmosDb)
    .WithReference(keyVault);

builder.AddProject<Projects.Sudoku_Web_Server>("sudoku-blazor")
    .WithReference(cosmosDb)
    .WithReference(keyVault)
    .WithExternalHttpEndpoints();

builder.Build().Run();
