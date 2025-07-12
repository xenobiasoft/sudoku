namespace Sudoku.Domain.Exceptions;

public class GameAlreadyCompletedException : DomainException
{
    public GameAlreadyCompletedException(string message) : base(message)
    {
    }
}