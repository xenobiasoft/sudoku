using DepenMock.Moq;
using MediatR;
using Sudoku.Application.DTOs;

namespace UnitTests.API;

public abstract class BaseGameControllerTests<TControllerType> : MoqBaseTestByType<TControllerType> where TControllerType : class
{
    protected readonly Mock<IMediator> MockMediator;
    protected TControllerType Sut;

    protected BaseGameControllerTests()
    {
        MockMediator = Container.ResolveMock<IMediator>().AsMoq();
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
