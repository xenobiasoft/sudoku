namespace Sudoku.Domain.Exceptions;

public class GameNotPausedException : DomainException
{
    public GameNotPausedException(string message) : base(message)
    {
    }
}