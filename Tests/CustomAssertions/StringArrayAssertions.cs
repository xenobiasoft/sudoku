using System.Diagnostics;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using XenobiaSoft.Sudoku.Helpers;

namespace UnitTests.CustomAssertions;

public class StringArrayAssertions : ReferenceTypeAssertions<string[,], StringArrayAssertions>
{
	public StringArrayAssertions(string[,] subject) : base(subject)
	{ }

	protected override string Identifier => "string array";

	[DebuggerStepThrough]
	public AndConstraint<StringArrayAssertions> BeEmpty(string because = "", params object[] becauseArgs)
	{
		Execute.Assertion
			.BecauseOf(because, becauseArgs)
			.ForCondition(Subject.IsEmpty())
			.FailWith($"{Identifier} is not empty");

		return new AndConstraint<StringArrayAssertions>(this);
	}

	[DebuggerStepThrough]
	public AndConstraint<StringArrayAssertions> BeEquivalentTo(string[,] target, string because = "", params object[] becauseArgs)
	{
		Execute.Assertion
			.BecauseOf(because, becauseArgs)
			.ForCondition(Subject.IsEquivalentTo(target))
			.FailWith($"{Identifier} does not match {target}.\r\n\r\nExpected:\r\n{target.ToJson()}\r\n\r\nBut found:\r\n{Subject.ToJson()}");

		return new AndConstraint<StringArrayAssertions>(this);
	}
}