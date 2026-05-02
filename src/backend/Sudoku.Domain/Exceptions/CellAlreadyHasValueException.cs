namespace Sudoku.Domain.Exceptions;

public class CellAlreadyHasValueException(string message) : DomainException(message);
