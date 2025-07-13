namespace Sudoku.Domain.Exceptions;

public class InvalidPlayerAliasException(string message) : DomainException(message);