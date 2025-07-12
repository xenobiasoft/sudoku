namespace Sudoku.Domain.Exceptions;

public class GameNotInStartStateException : DomainException
{
    public GameNotInStartStateException(string message) : base(message)
    {
    }
}