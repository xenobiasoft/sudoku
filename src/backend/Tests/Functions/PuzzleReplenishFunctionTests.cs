using Azure.Messaging.EventGrid;
using DepenMock.Attributes;
using DepenMock.Moq;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.ValueObjects;
using Sudoku.Functions.Functions;

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
    [InlineData("9x9", "easy", "Easy")]
    [InlineData("9x9", "medium", "Medium")]
    [InlineData("9x9", "hard", "Hard")]
    [InlineData("9x9", "expert", "Expert")]
    [InlineData("16x16", "easy", "Easy")]
    [InlineData("16x16", "expert", "Expert")]
    public async Task Run_WithValidBlobPath_ParsesSizeAndDifficultyAndSeedsOneReplacement(
        string sizeSegment, string difficultyPrefix, string expectedDifficultyName)
    {
        // Arrange
        var expectedSize = BoardSize.FromValue(int.Parse(sizeSegment.Split('x')[0]));
        var expectedDifficulty = GameDifficulty.FromName(expectedDifficultyName);
        var subject = $"/blobServices/default/containers/sudoku-puzzles/blobs/{sizeSegment}/{difficultyPrefix}/{Guid.NewGuid()}.json";
        var eventGridEvent = CreateEventGridEvent(subject);

        // Act
        await _sut.Run(eventGridEvent);

        // Assert
        _mockPuzzlePoolService.VerifySeedCalledOnce(expectedSize, expectedDifficulty, 1);
    }

    [Fact]
    public async Task Run_WithUnknownDifficultyName_DoesNotSeed()
    {
        // Arrange
        var subject = "/blobServices/default/containers/sudoku-puzzles/blobs/9x9/unknown/puzzle.json";
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
        var subject = "/blobServices/default/containers/sudoku-puzzles/blobs/9x9/unknown/puzzle.json";
        var eventGridEvent = CreateEventGridEvent(subject);

        // Act
        await _sut.Run(eventGridEvent);

        // Assert
        Logger.WarningLogs().AssertContains("Unknown difficulty");
    }

    [Fact]
    public async Task Run_WithUnknownSizeSegment_DoesNotSeed()
    {
        // Arrange
        var subject = "/blobServices/default/containers/sudoku-puzzles/blobs/25x25/easy/puzzle.json";
        var eventGridEvent = CreateEventGridEvent(subject);

        // Act
        await _sut.Run(eventGridEvent);

        // Assert
        _mockPuzzlePoolService.VerifySeedNeverCalled();
    }

    [Fact]
    public async Task Run_WithUnknownSizeSegment_LogsWarning()
    {
        // Arrange
        var subject = "/blobServices/default/containers/sudoku-puzzles/blobs/25x25/easy/puzzle.json";
        var eventGridEvent = CreateEventGridEvent(subject);

        // Act
        await _sut.Run(eventGridEvent);

        // Assert
        Logger.WarningLogs().AssertContains("Unknown board size");
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
