namespace Sudoku.Domain.Exceptions;

public class NoAvailableCellsForHintException() : DomainException("No empty cells available to reveal a hint");
