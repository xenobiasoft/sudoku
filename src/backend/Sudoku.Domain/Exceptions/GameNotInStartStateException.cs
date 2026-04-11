namespace Sudoku.Domain.Exceptions;

public class GameNotInStartStateException(string message) : DomainException(message);