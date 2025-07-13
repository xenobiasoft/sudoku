namespace Sudoku.Domain.Exceptions;

public class InvalidPuzzleException(string message) : DomainException(message);