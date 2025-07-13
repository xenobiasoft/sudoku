namespace Sudoku.Domain.Exceptions;

public class InvalidCellPositionException(string message) : DomainException(message);