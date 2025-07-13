namespace Sudoku.Domain.Exceptions;

public class GameAlreadyCompletedException(string message) : DomainException(message);