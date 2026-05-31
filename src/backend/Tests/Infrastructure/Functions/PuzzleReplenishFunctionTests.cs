using Azure.Messaging.EventGrid;
using DepenMock.Moq;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.ValueObjects;
using Sudoku.Functions.Functions;

namespace UnitTests.Infrastructure.Functions;

public class PuzzleReplenishFunctionTests : MoqBaseTestByType<PuzzleReplenishFunction>
{
    private readonly Mock<IPuzzlePoolService> _mockPuzzlePoolService;

    public PuzzleReplenishFunctionTests()
    {
        _mockPuzzlePoolService = Container.ResolveMock<IPuzzlePoolService>().AsMoq();
    }

    [Theory]
    [InlineData("easy", "Easy")]
    [InlineData("medium", "Medium")]
    [InlineData("hard", "Hard")]
    [InlineData("expert", "Expert")]
    public async Task Run_WithValidBlobPath_ParsesDifficultyAndSeedsOneReplacement(string difficultyPrefix, string expectedDifficultyName)
    {
        // Arrange
        var subject = $"/blobServices/default/containers/sudoku-puzzles/blobs/{difficultyPrefix}/{Guid.NewGuid()}.json";
        var eventGridEvent = CreateEventGridEvent(subject);

        _mockPuzzlePoolService
            .Setup(x => x.SeedAsync(It.IsAny<GameDifficulty>(), 1))
            .Returns(Task.CompletedTask);

        var sut = ResolveSut();

        // Act
        await sut.Run(eventGridEvent);

        // Assert
        _mockPuzzlePoolService.Verify(
            x => x.SeedAsync(It.Is<GameDifficulty>(d => d.Name == expectedDifficultyName), 1),
            Times.Once);
    }

    [Fact]
    public async Task Run_WithUnknownDifficultyName_LogsWarningAndDoesNotSeed()
    {
        // Arrange
        var subject = "/blobServices/default/containers/sudoku-puzzles/blobs/unknown/puzzle.json";
        var eventGridEvent = CreateEventGridEvent(subject);

        var sut = ResolveSut();

        // Act
        await sut.Run(eventGridEvent);

        // Assert
        _mockPuzzlePoolService.Verify(
            x => x.SeedAsync(It.IsAny<GameDifficulty>(), It.IsAny<int>()),
            Times.Never);
        Logger.WarningLogs().ContainsMessage("Unknown difficulty");
    }

    [Fact]
    public async Task Run_WithMalformedSubject_LogsWarningAndDoesNotSeed()
    {
        // Arrange
        var eventGridEvent = CreateEventGridEvent("/no-blobs-segment/here");

        var sut = ResolveSut();

        // Act
        await sut.Run(eventGridEvent);

        // Assert
        _mockPuzzlePoolService.Verify(
            x => x.SeedAsync(It.IsAny<GameDifficulty>(), It.IsAny<int>()),
            Times.Never);
    }

    private static EventGridEvent CreateEventGridEvent(string subject) =>
        new(subject, "Microsoft.Storage.BlobDeleted", "1.0", BinaryData.FromString("{}"));
}
