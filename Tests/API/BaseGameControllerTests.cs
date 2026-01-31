using DepenMock.XUnit;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;

namespace UnitTests.API;

public abstract class BaseGameControllerTests<TControllerType> : BaseTestByType<TControllerType> where TControllerType : class
{
    protected readonly Mock<IGameApplicationService> MockGameService;
    protected TControllerType Sut;

    protected BaseGameControllerTests()
    {
        MockGameService = Container.ResolveMock<IGameApplicationService>();
        Sut = ResolveSut();
    }
    
    protected static GameDto CreateTestGameDto(string playerAlias, string difficulty, string? gameId = null)
    {
        return new GameDto(
            gameId ?? Guid.NewGuid().ToString(),
            playerAlias,
            difficulty,
            "NotStarted",
            new GameStatisticsDto(0, 0, 0, TimeSpan.Zero, 0.0),
            DateTime.UtcNow,
            null,
            null,
            null,
            [],
            []
        );
    }
}