using Sudoku.Web.Server.Services.Abstractions.V2;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services.V2;

public partial class GameManager : IGameStatisticsManager
{
    public IGameStatistics CurrentSession => Game.Statistics;

    public async Task EndSession()
    {
        throw new NotImplementedException();
    }

    public async Task PauseSession()
    {
        throw new NotImplementedException();
    }

    public async Task RecordMove(bool isValid)
    {
        throw new NotImplementedException();
    }

    public async Task ResumeSession(GameStateMemory gameState)
    {
        throw new NotImplementedException();
    }

    public async Task StartNewSession(GameStateMemory gameState)
    {
        throw new NotImplementedException();
    }
}