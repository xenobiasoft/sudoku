namespace Sudoku.Infrastructure.Configuration;

public class CosmosDbOptions
{
    public const string SectionName = "CosmosDb";

    public string ContainerName { get; set; } = "games";
    public string DatabaseName { get; set; } = "sudoku";
    public bool DisableSslValidation { get; set; }
}