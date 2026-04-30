namespace Sudoku.Domain.ValueObjects;

public record ProfileId
{
    public Guid Value { get; }
    private ProfileId(Guid value) => Value = value;
    public static ProfileId New() => new(Guid.NewGuid());
    public static ProfileId From(Guid value) => new(value);
    public static ProfileId From(string value) => new(Guid.Parse(value));
    public override string ToString() => Value.ToString();
}
