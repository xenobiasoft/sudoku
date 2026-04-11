namespace Sudoku.Domain.Exceptions;

public class GameNotInProgressException(string message) : DomainException(message);