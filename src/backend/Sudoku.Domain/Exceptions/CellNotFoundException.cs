namespace Sudoku.Domain.Exceptions;

public class CellNotFoundException(string message) : DomainException(message);