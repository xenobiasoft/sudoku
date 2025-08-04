namespace Sudoku.Infrastructure.Configuration;

public class AzureStorageOptions
{
    public const string SectionName = "AzureStorage";

    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = "sudoku-games";
    public bool UseManagedIdentity { get; set; } = false;
    public string? AccountName { get; set; }
    public string? AccountKey { get; set; }
}