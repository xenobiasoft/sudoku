namespace XenobiaSoft.Sudoku.Exceptions;

public class InvalidMoveException : Exception
{
	public InvalidMoveException() : base("An invalid move was made")
	{ }
}