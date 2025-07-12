namespace Sudoku.Domain.Exceptions;

public class InvalidGameDifficultyException : DomainException
{
    public InvalidGameDifficultyException(string message) : base(message)
    {
    }
}