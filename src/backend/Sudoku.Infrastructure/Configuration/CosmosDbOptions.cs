using Microsoft.Azure.Cosmos;

namespace Sudoku.Infrastructure.Configuration;

public class CosmosDbOptions
{
    public const string SectionName = "CosmosDb";

    public string ContainerName { get; set; } = "games";
    public string DatabaseName { get; set; } = "sudoku";
    public bool DisableSslValidation { get; set; }
    public bool UseManagedIdentity { get; set; }
    public string? AccountEndpoint { get; set; }
    public ConnectionMode ConnectionMode { get; set; } = ConnectionMode.Gateway;
}