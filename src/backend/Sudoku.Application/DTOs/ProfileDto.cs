using Sudoku.Domain.Entities;

namespace Sudoku.Application.DTOs;

public record ProfileDto(string ProfileId, string Alias, DateTime CreatedAt, DateTime UpdatedAt)
{
    public static ProfileDto FromProfile(UserProfile profile) =>
        new(profile.Id.ToString(), profile.Alias.Value, profile.CreatedAt, profile.UpdatedAt);
}
