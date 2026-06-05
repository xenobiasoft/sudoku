using Sudoku.Infrastructure.Services;

namespace UnitTests.Mocks;

public static class MockAzureStorageServiceExtensions
{
    extension(Mock<IAzureStorageService> mock)
    {
        public void SetupGetBlobNamesReturns(IEnumerable<string> blobNames)
        {
            mock.Setup(x => x.GetBlobNamesAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(blobNames.ToAsyncEnumerable());
        }

        public void SetupLoadReturns<TBlobType>(TBlobType document)
        {
            mock.Setup(x => x.LoadAsync<TBlobType>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(document);
        }

        public void VerifyDeletesBlob(string containerName, string blobName)
        {
            mock.Verify(x => x.DeleteAsync(containerName, blobName), Times.Once);
        }

        public void VerifySavesBlob<TBlobType>(string containerName, string blobName)
        {
            mock.Verify(x => x.SaveAsync(containerName, blobName, It.IsAny<TBlobType>()), Times.Once);
        }

        public void VerifySavesBlobCheckUsingPartialName<TBlobType>(string containerName, string blobNamePrefix)
        {
            mock.Verify(x => x.SaveAsync(containerName, It.Is<string>(n => n.StartsWith(blobNamePrefix)), It.IsAny<TBlobType>()), Times.Once);
        }
    }
}