using Sudoku.Domain.Entities;
using Sudoku.Domain.Events;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Domain;

public class UserProfileTests
{
    [Fact]
    public void Create_WithValidAlias_SetsPropertiesCorrectly()
    {
        var alias = PlayerAlias.Create("testalias");
        var before = DateTime.UtcNow;

        var profile = UserProfile.Create(alias);

        profile.Id.Should().NotBeNull();
        profile.Alias.Should().Be(alias);
        profile.CreatedAt.Should().BeOnOrAfter(before);
        profile.UpdatedAt.Should().BeCloseTo(profile.CreatedAt, TimeSpan.FromMilliseconds(100));
        profile.LockedAt.Should().BeNull();
        profile.LockToken.Should().BeNull();
    }

    [Fact]
    public void Create_WithValidAlias_RaisesProfileCreatedEvent()
    {
        var alias = PlayerAlias.Create("testalias");

        var profile = UserProfile.Create(alias);

        profile.DomainEvents.Should().ContainSingle();
        var evt = profile.DomainEvents.First().Should().BeOfType<ProfileCreatedEvent>().Subject;
        evt.Alias.Should().Be(alias);
        evt.ProfileId.Should().Be(profile.Id);
    }

    [Fact]
    public void UpdateAlias_WithNewAlias_ChangesAliasAndRaisesEvent()
    {
        var originalAlias = PlayerAlias.Create("original");
        var newAlias = PlayerAlias.Create("updated");
        var profile = UserProfile.Create(originalAlias);
        profile.ClearDomainEvents();

        profile.UpdateAlias(newAlias);

        profile.Alias.Should().Be(newAlias);
        profile.DomainEvents.Should().ContainSingle();
        var evt = profile.DomainEvents.First().Should().BeOfType<ProfileAliasUpdatedEvent>().Subject;
        evt.OldAlias.Should().Be(originalAlias);
        evt.NewAlias.Should().Be(newAlias);
        evt.ProfileId.Should().Be(profile.Id);
    }

    [Fact]
    public void UpdateAlias_MultipleUpdates_EachRaisesEvent()
    {
        var alias1 = PlayerAlias.Create("alias1");
        var alias2 = PlayerAlias.Create("alias2");
        var alias3 = PlayerAlias.Create("alias3");
        var profile = UserProfile.Create(alias1);
        profile.ClearDomainEvents();

        profile.UpdateAlias(alias2);
        profile.UpdateAlias(alias3);

        profile.DomainEvents.Should().HaveCount(2);
        profile.Alias.Should().Be(alias3);
    }
}
