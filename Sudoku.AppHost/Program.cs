var builder = DistributedApplication.CreateBuilder(args);

var keyVault = builder.AddConnectionString("AzureKeyVault");

builder.AddProject<Projects.Sudoku_Api>("sudoku-api")
    .WithReference(keyVault);

builder.AddProject<Projects.Sudoku_Web_Server>("sudoku-blazor")
    .WithReference(keyVault)
    .WithExternalHttpEndpoints();

builder.Build().Run();
