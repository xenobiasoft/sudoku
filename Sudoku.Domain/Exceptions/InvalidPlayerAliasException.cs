namespace Sudoku.Domain.Exceptions;

public class InvalidPlayerAliasException : DomainException
{
    public InvalidPlayerAliasException(string message) : base(message)
    {
    }
}