using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Interfaces;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByAliasAsync(PlayerAlias alias);
    Task<UserProfile?> GetByIdAsync(ProfileId id);
    Task<bool> AliasExistsAsync(PlayerAlias alias);
    Task SaveAsync(UserProfile profile);
}
