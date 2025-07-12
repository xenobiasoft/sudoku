namespace Sudoku.Domain.Exceptions;

public class InvalidCellValueException : DomainException
{
    public InvalidCellValueException(string message) : base(message)
    {
    }
}