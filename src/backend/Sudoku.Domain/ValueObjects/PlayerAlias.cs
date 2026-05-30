namespace Sudoku.Domain.ValueObjects;

public record PlayerAlias
{
    public string Value { get; }

    private PlayerAlias(string value)
    {
        Value = value;
    }

    public static PlayerAlias Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidPlayerAliasException("Player alias cannot be null or empty");
        }

        var trimmed = value.Trim();

        if (trimmed.Length > 50)
        {
            throw new InvalidPlayerAliasException("Player alias cannot exceed 50 characters");
        }

        if (trimmed.Length < 2)
        {
            throw new InvalidPlayerAliasException("Player alias must be at least 2 characters");
        }

        if (!trimmed.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
        {
            throw new InvalidPlayerAliasException("Player alias can only contain letters, numbers, dashes, and underscores");
        }

        return new PlayerAlias(trimmed);
    }

    public static implicit operator string(PlayerAlias playerAlias) => playerAlias.Value;

    public override string ToString() => Value;
}