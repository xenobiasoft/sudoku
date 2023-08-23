using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using XenobiaSoft.Sudoku;

namespace UnitTests.CustomAssertions;

public class IntArrayAssertions : ReferenceTypeAssertions<int[,], IntArrayAssertions>
{
	public IntArrayAssertions(int[,] subject) : base(subject)
	{ }

	protected override string Identifier => "string array";

	public AndConstraint<IntArrayAssertions> BeEmpty(string because = "", params object[] becauseArgs)
	{
		Execute.Assertion
			.BecauseOf(because, becauseArgs)
			.ForCondition(Subject.IsEmpty())
			.FailWith($"{Identifier} is not empty");

		return new AndConstraint<IntArrayAssertions>(this);
	}
}