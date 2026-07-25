using System.Net;
using DepenMock.Attributes;
using DepenMock.Moq;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Sudoku.Domain.ValueObjects;
using Sudoku.Functions.Functions;
using Sudoku.Functions.Services;

namespace UnitTests.Functions;

[LogOutput(LogOutputTiming.Always)]
public class PuzzlePoolSeedHttpFunctionTests : MoqBaseTestByType<PuzzlePoolSeedHttpFunction>
{
    private readonly Mock<IPuzzlePoolSeeder> _mockSeeder;

    public PuzzlePoolSeedHttpFunctionTests()
    {
        _mockSeeder = Container.ResolveMock<IPuzzlePoolSeeder>().AsMoq();
    }

    [Fact]
    public async Task Run_InvokesSeederOnce()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        await sut.Run(CreateHttpRequest());

        // Assert
        _mockSeeder.VerifySeedPoolCalledOnce();
    }

    [Fact]
    public async Task Run_WithNoQueryParameters_SeedsTheWholeMatrix()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        await sut.Run(CreateHttpRequest());

        // Assert — a null filter means "top up every supported combination"
        _mockSeeder.VerifySeedPoolCalledWithoutFilter();
    }

    [Fact]
    public async Task Run_WithSizeAndDifficulty_PassesMatchingFilterToSeeder()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        await sut.Run(CreateHttpRequest("?size=16&difficulty=Easy"));

        // Assert
        _mockSeeder.VerifySeedPoolCalledWith(new PuzzlePoolSeedFilter(BoardSize.Sixteen, GameDifficulty.Easy));
    }

    [Fact]
    public async Task Run_WithCount_PassesTargetOverrideToSeeder()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        await sut.Run(CreateHttpRequest("?size=16&count=3"));

        // Assert
        _mockSeeder.VerifySeedPoolCalledWith(new PuzzlePoolSeedFilter(BoardSize.Sixteen, Target: 3));
    }

    [Theory]
    [InlineData("?size=12")]
    [InlineData("?size=abc")]
    [InlineData("?difficulty=Impossible")]
    [InlineData("?count=-1")]
    [InlineData("?count=notanumber")]
    public async Task Run_WithInvalidQueryParameter_ReturnsBadRequest(string query)
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var response = await sut.Run(CreateHttpRequest(query));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Run_WithInvalidQueryParameter_DoesNotInvokeSeeder()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        await sut.Run(CreateHttpRequest("?size=12"));

        // Assert
        _mockSeeder.VerifySeedPoolNeverCalled();
    }

    [Fact]
    public async Task Run_ReturnsOkStatus()
    {
        // Arrange
        var sut = ResolveSut();

        // Act
        var response = await sut.Run(CreateHttpRequest());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Run_WritesSeededCountToBody()
    {
        // Arrange
        _mockSeeder.SetupSeedPoolReturns(5);
        var sut = ResolveSut();

        // Act
        var response = await sut.Run(CreateHttpRequest());

        // Assert
        ReadBody(response).Should().Contain("\"seeded\":5");
    }

    private static HttpRequestData CreateHttpRequest(string query = "")
    {
        var context = new Mock<FunctionContext>().Object;
        var request = new Mock<HttpRequestData>(context);
        var response = new Mock<HttpResponseData>(context);

        response.SetupProperty(r => r.StatusCode);
        response.Setup(r => r.Headers).Returns(new HttpHeadersCollection());
        response.Setup(r => r.Body).Returns(new MemoryStream());
        request.Setup(r => r.CreateResponse()).Returns(response.Object);
        request.Setup(r => r.Url).Returns(new Uri($"http://localhost/api/seed-puzzle-pool{query}"));

        return request.Object;
    }

    private static string ReadBody(HttpResponseData response)
    {
        response.Body.Position = 0;
        using var reader = new StreamReader(response.Body);
        return reader.ReadToEnd();
    }
}
