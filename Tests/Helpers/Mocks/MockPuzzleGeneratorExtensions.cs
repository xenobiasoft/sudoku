using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.Abstractions;

namespace UnitTests.Helpers.Mocks;

public static class MockPuzzleGeneratorExtensions
{
    public static Mock<IPuzzleGenerator> SetupGenerate(this Mock<IPuzzleGenerator> mock, ISudokuPuzzle puzzle)
    {
        mock
            .Setup(x => x.GenerateAsync(It.IsAny<GameDifficulty>()))
            .ReturnsAsync(puzzle);

        return mock;
    }

    public static void VerifyGenerateCalled(this Mock<IPuzzleGenerator> mock, GameDifficulty difficulty, Func<Times> times)
    {
        mock.Verify(x => x.GenerateAsync(difficulty), times);
    }
}