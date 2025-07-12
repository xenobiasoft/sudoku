namespace Sudoku.Domain.Exceptions;

public class InvalidCellPositionException : DomainException
{
    public InvalidCellPositionException(string message) : base(message)
    {
    }
}