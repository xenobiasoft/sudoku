using XenobiaSoft.Sudoku.GameState;

namespace XenobiaSoft.Sudoku.Abstractions;

public interface IStorageService
{
    Task DeleteAsync(string containerName, string blobName);
    IAsyncEnumerable<string> GetBlobNamesAsync(string containerName, string blobPrefix);
    Task<TBlobType> LoadAsync<TBlobType>(string containerName, string blobName) where TBlobType : class;
    Task SaveAsync(string containerName, string blobName, object blob, bool? overwrite = false);
}