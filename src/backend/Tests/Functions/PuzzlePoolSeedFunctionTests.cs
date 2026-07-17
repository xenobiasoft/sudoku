using DepenMock.Attributes;
using DepenMock.Moq;
using Microsoft.Azure.Functions.Worker;
using Sudoku.Functions.Functions;
using Sudoku.Functions.Services;

namespace UnitTests.Functions;

[LogOutput(LogOutputTiming.Always)]
public class PuzzlePoolSeedFunctionTests : MoqBaseTestByType<PuzzlePoolSeedFunction>
{
    private readonly Mock<IPuzzlePoolSeeder> _mockSeeder;

    public PuzzlePoolSeedFunctionTests()
    {
        _mockSeeder = Container.ResolveMock<IPuzzlePoolSeeder>().AsMoq();
    }

    [Fact]
    public async Task Run_InvokesSeederOnce()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        await sut.Run(CreateTimerInfo());

        // Assert
        _mockSeeder.VerifySeedPoolCalledOnce();
    }

    [Fact]
    public async Task Run_LogsTriggerInformation()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        await sut.Run(CreateTimerInfo());

        // Assert
        Logger.InformationLogs().ContainsMessage("Puzzle pool seed function triggered");
    }

    private static TimerInfo CreateTimerInfo() => new();
}
