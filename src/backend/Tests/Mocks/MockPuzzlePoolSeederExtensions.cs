using Sudoku.Functions.Services;

namespace UnitTests.Mocks;

public static class MockPuzzlePoolSeederExtensions
{
    extension(Mock<IPuzzlePoolSeeder> mock)
    {
        public void SetupSeedPoolReturns(int seeded)
        {
            mock.Setup(x => x.SeedPoolAsync()).ReturnsAsync(seeded);
        }

        public void VerifySeedPoolCalledOnce()
        {
            mock.Verify(x => x.SeedPoolAsync(), Times.Once);
        }
    }
}
