namespace Sudoku.Domain.Exceptions;

public class CellIsFixedException(string message) : DomainException(message);