using Sudoku.Web.Server.Services.Abstractions;

namespace Sudoku.Web.Server.Services;

public class AliasService(ILocalStorageService localStorageService) : IAliasService
{
    public async Task<string> GetAliasAsync()
    {
        var alias = await localStorageService.GetAliasAsync();

        if (!string.IsNullOrEmpty(alias)) return alias;

        alias = AliasGenerator.GenerateAlias();
        await localStorageService.SetAliasAsync(alias);

        return alias;
    }
}