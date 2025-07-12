namespace Sudoku.Domain.Exceptions;

public class CellNotFoundException : DomainException
{
    public CellNotFoundException(string message) : base(message)
    {
    }
}