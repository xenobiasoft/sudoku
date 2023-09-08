using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using XenobiaSoft.Sudoku;

namespace UnitTests.CustomAssertions;

public class SudokuPuzzleAssertions : ReferenceTypeAssertions<Cell[], SudokuPuzzleAssertions>
{
    public SudokuPuzzleAssertions(Cell[] instance) : base(instance)
    { }

    protected override string Identifier => "puzzle";

    public AndConstraint<SudokuPuzzleAssertions> BeEmpty(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.All(x => !x.Value.HasValue))
            .FailWith($"{Identifier} values is not empty")
            .Then
            .ForCondition(Subject.All(x => string.IsNullOrWhiteSpace(x.PossibleValues)))
            .FailWith($"{Identifier} possible values is not empty");

        return new AndConstraint<SudokuPuzzleAssertions>(this);
    }

    public AndConstraint<SudokuPuzzleAssertions> BeEquivalentTo(Cell[] cells, string because = "",
	    params object[] becauseArgs)
	{
		Execute.Assertion
			.BecauseOf(because, becauseArgs)
			.ForCondition(Subject.SequenceEqual(cells))
			.FailWith($"Expected:\r\n{Identifier.ToJson()}\r\n\r\nTo be equivalent to:{cells.ToJson()}");

		return new AndConstraint<SudokuPuzzleAssertions>(this);
	}
}