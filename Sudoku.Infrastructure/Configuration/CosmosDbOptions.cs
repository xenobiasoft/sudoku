namespace Sudoku.Infrastructure.Configuration;

public class CosmosDbOptions
{
    public const string SectionName = "CosmosDb";
    
    public string DatabaseName { get; set; } = "SudokuDb";
    public string ContainerName { get; set; } = "Games";
}