using Sudoku.Application.Models;
using Sudoku.Domain.ValueObjects;

namespace Sudoku.Application.Interfaces;

public interface IGameCompletionRepository
{
    Task<IEnumerable<GameCompletion>> GetByProfileIdAsync(ProfileId profileId);
    Task<GameCompletion?> GetByGameIdAsync(GameId gameId, ProfileId profileId);

    /// <summary>
    /// Idempotent upsert keyed by <see cref="GameCompletion.GameId"/>. A duplicate completion
    /// event or the delete-time backstop writing the same game must not double-count.
    /// </summary>
    Task UpsertAsync(GameCompletion completion);
}
