namespace Sudoku.Domain.Exceptions;

public class NoMoveHistoryException() : DomainException("No moves to undo");