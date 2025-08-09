namespace Sudoku.Infrastructure.Configuration;

public class CosmosDbOptions
{
    public const string SectionName = "CosmosDb";
    
    public string DatabaseName { get; set; } = "sudoku";
    public string ContainerName { get; set; } = "games";
}