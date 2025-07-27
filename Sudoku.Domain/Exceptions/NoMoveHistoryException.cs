namespace Sudoku.Domain.Exceptions;

public class NoMoveHistoryException : DomainException
{
    public NoMoveHistoryException(string message) : base(message)
    {
    }
}