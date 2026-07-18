using DepenMock.Attributes;
using DepenMock.Moq;
using MediatR;
using Sudoku.Application.DTOs;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.API;

[LogOutput(LogOutputTiming.Always)]
public abstract class BaseGameControllerTests<TControllerType> : MoqBaseTestByType<TControllerType> where TControllerType : class
{
    protected readonly Mock<IMediator> MockMediator;
    protected TControllerType Sut;

    protected BaseGameControllerTests()
    {
        MockMediator = Container.ResolveMock<IMediator>().AsMoq();
        Sut = ResolveSut();
    }

    protected static GameDto CreateTestGameDto(string profileId, string difficulty, string? gameId = null)
    {
        return new GameDto(
            gameId ?? Guid.NewGuid().ToString(),
            profileId,
            "TestPlayer",
            difficulty,
            "NotStarted",
            new GameStatisticsDto(0, 0, 0, 0, BoardSize.Nine.MaxHints, TimeSpan.Zero, 0.0),
            DateTime.UtcNow,
            null,
            null,
            null,
            [],
            []
        );
    }
}
