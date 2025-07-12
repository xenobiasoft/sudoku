namespace Sudoku.Domain.Exceptions;

public class GameNotInProgressException : DomainException
{
    public GameNotInProgressException(string message) : base(message)
    {
    }
}