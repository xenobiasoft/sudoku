namespace Sudoku.Domain.Exceptions;

public class InvalidGameDifficultyException(string message) : DomainException(message);