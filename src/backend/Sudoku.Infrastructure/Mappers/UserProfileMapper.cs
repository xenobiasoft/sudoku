using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Models;

namespace Sudoku.Infrastructure.Mappers;

public static class UserProfileMapper
{
    public static UserProfileDocument ToDocument(UserProfile profile)
    {
        return new UserProfileDocument
        {
            Id = profile.Id.ToString(),
            ProfileId = profile.Id.ToString(),
            Alias = profile.Alias.Value,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt,
            LockedAt = profile.LockedAt,
        };
    }

    public static UserProfile ToDomain(UserProfileDocument document)
    {
        var profileId = ProfileId.From(document.ProfileId);
        var alias = PlayerAlias.Create(document.Alias);

        return CreateReconstituted(profileId, alias, document.CreatedAt, document.UpdatedAt, document.LockedAt);
    }

    private static UserProfile CreateReconstituted(
        ProfileId id,
        PlayerAlias alias,
        DateTime createdAt,
        DateTime updatedAt,
        DateTime? lockedAt)
    {
        var profile = (UserProfile)Activator.CreateInstance(typeof(UserProfile), nonPublic: true)!;

        SetPrivateField(profile, "<Id>k__BackingField", id);
        SetPrivateField(profile, "<Alias>k__BackingField", alias);
        SetPrivateField(profile, "<CreatedAt>k__BackingField", createdAt);
        SetPrivateField(profile, "<UpdatedAt>k__BackingField", updatedAt);
        SetPrivateField(profile, "<LockedAt>k__BackingField", lockedAt);

        return profile;
    }

    private static void SetPrivateField(object obj, string fieldName, object? value)
    {
        var type = obj.GetType();
        while (type != null)
        {
            var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(obj, value);
                return;
            }
            type = type.BaseType;
        }
    }
}
