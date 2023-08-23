using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using XenobiaSoft.Sudoku;

namespace UnitTests.CustomAssertions;

public class StringArrayAssertions : ReferenceTypeAssertions<string[,], StringArrayAssertions>
{
	public StringArrayAssertions(string[,] subject) : base(subject)
	{ }

	protected override string Identifier => "string array";

	public AndConstraint<StringArrayAssertions> BeEmpty(string because = "", params object[] becauseArgs)
	{
		Execute.Assertion
			.BecauseOf(because, becauseArgs)
			.ForCondition(Subject.IsEmpty())
			.FailWith($"{Identifier} is not empty");

		return new AndConstraint<StringArrayAssertions>(this);
	}

	public AndConstraint<StringArrayAssertions> BeEquivalentTo(string[,] target, string because = "", params object[] becauseArgs)
	{
		Execute.Assertion
			.BecauseOf(because, becauseArgs)
			.ForCondition(Subject.IsEquivalentTo(target))
			.FailWith($"{Identifier} does not match {target}");

		return new AndConstraint<StringArrayAssertions>(this);
	}
}