namespace Sudoku.Domain.Exceptions;

public class GameNotPausedException(string message) : DomainException(message);