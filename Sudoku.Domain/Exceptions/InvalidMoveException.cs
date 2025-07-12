namespace Sudoku.Domain.Exceptions;

public class InvalidMoveException : DomainException
{
    public InvalidMoveException(string message) : base(message)
    {
    }
}