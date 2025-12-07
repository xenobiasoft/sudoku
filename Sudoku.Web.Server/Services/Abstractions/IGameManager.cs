namespace Sudoku.Web.Server.Services.Abstractions;

public interface IGameManager : IGameStatisticsManager, IGameStateManager
{
    Task StartGameAsync();
}