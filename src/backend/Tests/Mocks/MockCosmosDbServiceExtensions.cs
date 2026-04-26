using Sudoku.Infrastructure.Models;
using Sudoku.Infrastructure.Services;

namespace UnitTests.Mocks;

public static class MockCosmosDbServiceExtensions
{
    extension(Mock<ICosmosDbService> mock)
    {
        public void ExistsAsyncReturns(string gameId, bool exists)
        {
            mock
                .Setup(x => x.ExistsAsync<SudokuGameDocument>(It.Is<string>(id => id == gameId), It.IsAny<string>()))
                .ReturnsAsync(exists);
        }

        public void GetItemReturnsDocument(string gameId, SudokuGameDocument document)
        {
            mock
                .Setup(x => x.GetItemAsync<SudokuGameDocument>(It.Is<string>(id => id == gameId), It.IsAny<string>()))
                .ReturnsAsync(document);
        }

        public void GetItemReturnsNothing(string gameId)
        {
            mock
                .Setup(x => x.GetItemAsync<SudokuGameDocument>(It.Is<string>(id => id == gameId), It.IsAny<string>()))
                .ReturnsAsync((SudokuGameDocument?)null);
        }

        public void GetByPlayerAsyncReturnsDocuments(string gameId, IEnumerable<SudokuGameDocument> documents)
        {
            mock
                .Setup(x => x.QueryItemsAsync<SudokuGameDocument>(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()))
                .ReturnsAsync(documents);
        }

        public void VerifyDeleteItemAsync(string gameId, Func<Times> times)
        {
            mock.Verify(x => x.DeleteItemAsync<SudokuGameDocument>(
                It.Is<string>(id => id == gameId),
                It.IsAny<string>()), times);
        }

        public void VerifyUpsertItemAsync(string gameId, string playerAlias, string difficulty, Func<Times> times)
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
}