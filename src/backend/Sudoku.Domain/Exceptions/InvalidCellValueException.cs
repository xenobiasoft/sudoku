namespace Sudoku.Domain.Exceptions;

public class InvalidCellValueException(string message) : DomainException(message);