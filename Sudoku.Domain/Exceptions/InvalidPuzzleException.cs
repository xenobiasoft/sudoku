namespace Sudoku.Domain.Exceptions;

public class InvalidPuzzleException : DomainException
{
    public InvalidPuzzleException(string message) : base(message)
    {
    }
}