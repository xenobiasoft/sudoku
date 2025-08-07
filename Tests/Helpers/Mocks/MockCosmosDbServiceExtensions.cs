using Sudoku.Infrastructure.Models;
using Sudoku.Infrastructure.Services;

namespace UnitTests.Helpers.Mocks;

public static class MockCosmosDbServiceExtensions
{
    public static void ExistsAsyncReturns(this Mock<ICosmosDbService> mock, string gameId, bool exists)
    {
        mock
            .Setup(x => x.ExistsAsync<SudokuGameDocument>(It.Is<string>(id => id == gameId), It.IsAny<string>()))
            .ReturnsAsync(exists);
    }

    public static void GetItemReturnsDocument(this Mock<ICosmosDbService> mock, string gameId, SudokuGameDocument document)
    {
        mock
            .Setup(x => x.GetItemAsync<SudokuGameDocument>(It.Is<string>(id => id == gameId), It.IsAny<string>()))
            .ReturnsAsync(document);
    }

    public static void GetItemReturnsNothing(this Mock<ICosmosDbService> mock, string gameId)
    {
        mock
            .Setup(x => x.GetItemAsync<SudokuGameDocument>(It.Is<string>(id => id == gameId), It.IsAny<string>()))
            .ReturnsAsync((SudokuGameDocument?)null);
    }

    public static void GetByPlayerAsyncReturnsDocuments(this Mock<ICosmosDbService> mock, string gameId, IEnumerable<SudokuGameDocument> documents)
    {
        mock
            .Setup(x => x.QueryItemsAsync<SudokuGameDocument>(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()))
            .ReturnsAsync(documents);
    }

    public static void VerifyDeleteItemAsync(this Mock<ICosmosDbService> mock, string gameId, Func<Times> times)
    {
        mock.Verify(x => x.DeleteItemAsync<SudokuGameDocument>(
            It.Is<string>(id => id == gameId),
            It.IsAny<string>()), times);
    }

    public static void VerifyUpsertItemAsync(this Mock<ICosmosDbService> mock, string gameId, string playerAlias, string difficulty, Func<Times> times)
    {
        mock.Verify(x => x.UpsertItemAsync(
                It.Is<SudokuGameDocument>(doc => 
                    doc.GameId == gameId && 
                    doc.PlayerAlias == playerAlias && 
                    doc.Difficulty == difficulty),
                It.IsAny<string?>()),
            times);
    }
}