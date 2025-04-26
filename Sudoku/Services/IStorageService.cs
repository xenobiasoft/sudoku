using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku.Services;

public interface IStorageService
{
    Task DeleteAsync(string containerName, string blobName);
    Task<TBlobType> LoadAsync<TBlobType>(string containerName, string blobName) where TBlobType : class;
    Task SaveAsync(string containerName, string blobName, object blob, CancellationToken token);
}