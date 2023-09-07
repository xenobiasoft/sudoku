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
            .ForCondition(Subject.Cells.All(x => !x.Value.HasValue))
            .FailWith($"{Identifier} values is not empty")
            .Then
            .ForCondition(Subject.Cells.All(x => string.IsNullOrWhiteSpace(x.PossibleValues)))
            .FailWith($"{Identifier} possible values is not empty");

        return new AndConstraint<SudokuPuzzleAssertions>(this);
    }
}