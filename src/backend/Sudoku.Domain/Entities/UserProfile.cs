namespace Sudoku.Domain.Entities;

public class UserProfile : AggregateRoot
{
    public ProfileId Id { get; private set; } = null!;
    public PlayerAlias Alias { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? LockedAt { get; private set; }   // reserved for #212
    public string? LockToken { get; private set; }    // reserved for #212, stored hashed

    private UserProfile() { }

    public static UserProfile Create(PlayerAlias alias)
    {
        var profile = new UserProfile
        {
            Id = ProfileId.New(),
            Alias = alias,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        profile.AddDomainEvent(new ProfileCreatedEvent(profile.Id, alias));
        return profile;
    }

    public void UpdateAlias(PlayerAlias newAlias)
    {
        var old = Alias;
        Alias = newAlias;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ProfileAliasUpdatedEvent(Id, old, newAlias));
    }
}
