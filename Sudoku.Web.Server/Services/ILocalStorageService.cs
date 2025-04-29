using Sudoku.Web.Server.Models;

namespace Sudoku.Web.Server.Services;

public interface ILocalStorageService
{
    Task<List<SavedGame>> GetSavedGamesAsync();
    Task SaveGameAsync(SavedGame game);
    Task ClearSavedGamesAsync();
}