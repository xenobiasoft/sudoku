using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Helpers.Factories;

public static class UserProfileFactory
{
    public static UserProfile CreateProfile(string alias = "TestPlayer")
    {
        return UserProfile.Create(PlayerAlias.Create(alias));
    }
}
