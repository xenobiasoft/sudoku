namespace Sudoku.Domain.ValueObjects;

public record GameId
{
    public Guid Value { get; }

    private GameId(Guid value)
    {
        Value = value;
    }

    public static GameId New() => new(Guid.NewGuid());
    public static GameId Create(Guid value) => new(value);
    public static GameId Create(string value) => new(Guid.Parse(value));

    public static implicit operator Guid(GameId gameId) => gameId.Value;
    public static implicit operator string(GameId gameId) => gameId.Value.ToString();

    public override string ToString() => Value.ToString();
}