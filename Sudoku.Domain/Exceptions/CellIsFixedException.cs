namespace Sudoku.Domain.Exceptions;

public class CellIsFixedException : DomainException
{
    public CellIsFixedException(string message) : base(message)
    {
    }
}