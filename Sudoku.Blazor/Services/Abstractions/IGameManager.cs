namespace Sudoku.Blazor.Services.Abstractions;

public interface IGameManager : IGameStatisticsManager, IGameStateManager
{
    Task StartGameAsync();
}