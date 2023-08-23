using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using XenobiaSoft.Sudoku;

namespace UnitTests.CustomAssertions;

public class SudokuPuzzleAssertions : ReferenceTypeAssertions<SudokuPuzzle, SudokuPuzzleAssertions>
{
    public SudokuPuzzleAssertions(SudokuPuzzle instance) : base(instance)
    { }

    protected override string Identifier => "puzzle";

    public AndConstraint<SudokuPuzzleAssertions> BeEmpty(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.Values.IsEmpty())
            .FailWith($"{Identifier} values is not empty")
            .Then
            .ForCondition(Subject.PossibleValues.IsEmpty())
            .FailWith($"{Identifier} possible values is not empty");

        return new AndConstraint<SudokuPuzzleAssertions>(this);
    }
}