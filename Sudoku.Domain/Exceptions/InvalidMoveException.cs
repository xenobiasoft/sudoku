namespace Sudoku.Domain.Exceptions;

public class InvalidMoveException(string message) : DomainException(message);