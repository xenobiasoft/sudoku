using DepenMock.XUnit;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Sudoku.Infrastructure.Configuration;
using Sudoku.Infrastructure.Services;
using System.Net;
using CosmosContainer = Microsoft.Azure.Cosmos.Container;

namespace UnitTests.Infrastructure.Services;

public class CosmosDbServiceTests : BaseTestByAbstraction<CosmosDbService, ICosmosDbService>
{
    private readonly Mock<CosmosClient> _mockCosmosClient;
    private readonly Mock<Database> _mockDatabase;
    private readonly Mock<CosmosContainer> _mockContainer;
    private readonly Mock<IOptions<CosmosDbOptions>> _mockOptions;

    public CosmosDbServiceTests()
    {
        _mockCosmosClient = Container.ResolveMock<CosmosClient>();
        _mockDatabase = new Mock<Database>();
        _mockContainer = new Mock<CosmosContainer>();
        _mockOptions = Container.ResolveMock<IOptions<CosmosDbOptions>>();

        var options = new CosmosDbOptions
        {
            DatabaseName = "TestDatabase",
            ContainerName = "TestContainer"
        };

        _mockOptions.Setup(x => x.Value).Returns(options);
        _mockCosmosClient.Setup(x => x.GetDatabase(It.IsAny<string>())).Returns(_mockDatabase.Object);
        _mockDatabase.Setup(x => x.GetContainer(It.IsAny<string>())).Returns(_mockContainer.Object);
    }

    [Fact]
    public async Task GetItemAsync_WhenItemExists_ShouldReturnItem()
    {
        // Arrange
        var testItem = new TestDocument { Id = "test-id", Name = "Test" };
        var itemResponse = new Mock<ItemResponse<TestDocument>>();
        itemResponse.Setup(x => x.Resource).Returns(testItem);

        _mockContainer
            .Setup(x => x.ReadItemAsync<TestDocument>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemResponse.Object);

        var sut = ResolveSut();

        // Act
        var result = await sut.GetItemAsync<TestDocument>("test-id", new PartitionKey("test-partition"));

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("test-id");
        result.Name.Should().Be("Test");
    }

    [Fact]
    public async Task GetItemAsync_WhenItemDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _mockContainer
            .Setup(x => x.ReadItemAsync<TestDocument>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CosmosException("Not found", HttpStatusCode.NotFound, 0, "", 0));

        var sut = ResolveSut();

        // Act
        var result = await sut.GetItemAsync<TestDocument>("non-existent-id", new PartitionKey("test-partition"));

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpsertItemAsync_ShouldCallContainerUpsert()
    {
        // Arrange
        var testItem = new TestDocument { Id = "test-id", Name = "Test" };
        var itemResponse = new Mock<ItemResponse<TestDocument>>();
        itemResponse.Setup(x => x.Resource).Returns(testItem);
        itemResponse.Setup(x => x.RequestCharge).Returns(5.0);

        _mockContainer
            .Setup(x => x.UpsertItemAsync(
                It.IsAny<TestDocument>(),
                It.IsAny<PartitionKey?>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemResponse.Object);

        var sut = ResolveSut();

        // Act
        var result = await sut.UpsertItemAsync(testItem, new PartitionKey("test-partition"));

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("test-id");
        _mockContainer.Verify(x => x.UpsertItemAsync(
            It.Is<TestDocument>(item => item.Id == "test-id"),
            It.IsAny<PartitionKey?>(),
            It.IsAny<ItemRequestOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExistsAsync_WhenItemExists_ShouldReturnTrue()
    {
        // Arrange
        var testItem = new TestDocument { Id = "test-id", Name = "Test" };
        var itemResponse = new Mock<ItemResponse<TestDocument>>();
        itemResponse.Setup(x => x.Resource).Returns(testItem);

        _mockContainer
            .Setup(x => x.ReadItemAsync<TestDocument>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemResponse.Object);

        var sut = ResolveSut();

        // Act
        var result = await sut.ExistsAsync<TestDocument>("test-id", new PartitionKey("test-partition"));

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenItemDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        _mockContainer
            .Setup(x => x.ReadItemAsync<TestDocument>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CosmosException("Not found", HttpStatusCode.NotFound, 0, "", 0));

        var sut = ResolveSut();

        // Act
        var result = await sut.ExistsAsync<TestDocument>("non-existent-id", new PartitionKey("test-partition"));

        // Assert
        result.Should().BeFalse();
    }

    public class TestDocument
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}