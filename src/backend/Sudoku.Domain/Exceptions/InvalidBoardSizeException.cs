namespace Sudoku.Domain.Exceptions;

public class InvalidBoardSizeException(string message) : DomainException(message);
