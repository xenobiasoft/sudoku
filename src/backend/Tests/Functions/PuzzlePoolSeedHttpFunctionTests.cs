using System.Net;
using DepenMock.Moq;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Sudoku.Functions.Functions;
using Sudoku.Functions.Services;

namespace UnitTests.Functions;

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

    private static HttpRequestData CreateHttpRequest()
    {
        var context = new Mock<FunctionContext>().Object;
        var request = new Mock<HttpRequestData>(context);
        var response = new Mock<HttpResponseData>(context);

        response.SetupProperty(r => r.StatusCode);
        response.Setup(r => r.Headers).Returns(new HttpHeadersCollection());
        response.Setup(r => r.Body).Returns(new MemoryStream());
        request.Setup(r => r.CreateResponse()).Returns(response.Object);

        return request.Object;
    }

    private static string ReadBody(HttpResponseData response)
    {
        response.Body.Position = 0;
        using var reader = new StreamReader(response.Body);
        return reader.ReadToEnd();
    }
}
