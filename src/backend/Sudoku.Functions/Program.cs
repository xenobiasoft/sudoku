using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sudoku.Functions.Services;
using Sudoku.Infrastructure.Configuration;

var builder = FunctionsApplication.CreateBuilder(args);
builder.AddAzureCosmosClient("CosmosDb");
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddScoped<IPuzzlePoolSeeder, PuzzlePoolSeeder>();
builder.Build().Run();
