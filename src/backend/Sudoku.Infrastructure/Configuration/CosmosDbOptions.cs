namespace Sudoku.Infrastructure.Configuration;

public class CosmosDbOptions
{
    public const string SectionName = "CosmosDb";

    public string ContainerName { get; set; } = "games";
    public string DatabaseName { get; set; } = "sudoku";

    /// <summary>
    /// Does NOT select credentials. Cosmos authentication comes entirely from the
    /// connection string that <c>AddAzureCosmosClient("CosmosDb")</c> resolves. This
    /// flag only suppresses container auto-creation, which the data-plane RBAC used
    /// with managed identity is not permitted to perform.
    /// </summary>
    public bool UseManagedIdentity { get; set; }

    public bool AutoCreateContainers { get; set; }
    public string ContainerPartitionKeyPath { get; set; } = "/id";
}
