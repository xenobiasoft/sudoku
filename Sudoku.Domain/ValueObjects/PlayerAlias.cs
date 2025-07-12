using Sudoku.Domain.Exceptions;

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
            throw new InvalidPlayerAliasException("Player alias cannot be null or empty");

        if (value.Length > 50)
            throw new InvalidPlayerAliasException("Player alias cannot exceed 50 characters");

        if (value.Length < 2)
            throw new InvalidPlayerAliasException("Player alias must be at least 2 characters");

        // Only allow alphanumeric characters and spaces
        if (!value.All(c => char.IsLetterOrDigit(c) || c == ' '))
            throw new InvalidPlayerAliasException("Player alias can only contain letters, numbers, and spaces");

        return new PlayerAlias(value.Trim());
    }

    public static implicit operator string(PlayerAlias playerAlias) => playerAlias.Value;

    public override string ToString() => Value;
}