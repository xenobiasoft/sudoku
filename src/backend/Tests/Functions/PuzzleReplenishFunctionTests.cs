using Azure.Messaging.EventGrid;
using DepenMock.Attributes;
using DepenMock.Moq;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.ValueObjects;
using Sudoku.Functions.Functions;
using LogOutputAttribute = DepenMock.XUnit.V3.Attributes.LogOutputAttribute;

namespace UnitTests.Functions;

[LogOutput(LogOutputTiming.Always)]
public class PuzzleReplenishFunctionTests : MoqBaseTestByType<PuzzleReplenishFunction>
{
    private readonly Mock<IPuzzlePoolService> _mockPuzzlePoolService;

    private readonly PuzzleReplenishFunction _sut;

    public PuzzleReplenishFunctionTests()
    {
        _mockPuzzlePoolService = Container.ResolveMock<IPuzzlePoolService>().AsMoq();

        _sut = ResolveSut();
    }

    [Theory]
    [InlineData("easy", "Easy")]
    [InlineData("medium", "Medium")]
    [InlineData("hard", "Hard")]
    [InlineData("expert", "Expert")]
    public async Task Run_WithValidBlobPath_ParsesDifficultyAndSeedsOneReplacement(string difficultyPrefix, string expectedDifficultyName)
    {
        // Arrange
        var expectedDifficulty = GameDifficulty.FromName(expectedDifficultyName);
        var subject = $"/blobServices/default/containers/sudoku-puzzles/blobs/{difficultyPrefix}/{Guid.NewGuid()}.json";
        var eventGridEvent = CreateEventGridEvent(subject);

        // Act
        await _sut.Run(eventGridEvent);

        // Assert
        _mockPuzzlePoolService.VerifySeedCalledOnce(expectedDifficulty, 1);
    }

    [Fact]
    public async Task Run_WithUnknownDifficultyName_DoesNotSeed()
    {
        // Arrange
        var subject = "/blobServices/default/containers/sudoku-puzzles/blobs/unknown/puzzle.json";
        var eventGridEvent = CreateEventGridEvent(subject);

        // Act
        await _sut.Run(eventGridEvent);

        // Assert
        _mockPuzzlePoolService.VerifySeedNeverCalled();
    }

    [Fact]
    public async Task Run_WithUnknownDifficultyName_LogsWarning()
    {
        // Arrange
        var subject = "/blobServices/default/containers/sudoku-puzzles/blobs/unknown/puzzle.json";
        var eventGridEvent = CreateEventGridEvent(subject);

        // Act
        await _sut.Run(eventGridEvent);

        // Assert
        Logger.WarningLogs().AssertContains("Unknown difficulty");
    }

    [Fact]
    public async Task Run_WithMalformedSubject_DoesNotSeed()
    {
        // Arrange
        var eventGridEvent = CreateEventGridEvent("/no-blobs-segment/here");

        // Act
        await _sut.Run(eventGridEvent);

        // Assert
        _mockPuzzlePoolService.VerifySeedNeverCalled();
    }

    [Fact]
    public async Task Run_WithMalformedSubject_LogsWarning()
    {
        // Arrange
        var eventGridEvent = CreateEventGridEvent("/no-blobs-segment/here");

        // Act
        await _sut.Run(eventGridEvent);

        // Assert
        Logger.WarningLogs().AssertContains("Could not parse difficulty from event subject: /no-blobs-segment/here");
    }

    private static EventGridEvent CreateEventGridEvent(string subject) =>
        new(subject, "Microsoft.Storage.BlobDeleted", "1.0", BinaryData.FromString("{}"));
}
