using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Generator;

namespace UnitTests.Helpers.Mocks;

public static class MockPuzzleGeneratorExtensions
{
    public static Mock<IPuzzleGenerator> SetupGenerate(this Mock<IPuzzleGenerator> mock, ISudokuPuzzle puzzle)
    {
        mock
            .Setup(x => x.Generate(It.IsAny<Level>()))
            .ReturnsAsync(puzzle);

        return mock;
    }
}