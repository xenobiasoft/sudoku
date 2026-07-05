namespace Sudoku.Domain.Exceptions;

public class HintLimitReachedException() : DomainException("No hints remaining for this game");
