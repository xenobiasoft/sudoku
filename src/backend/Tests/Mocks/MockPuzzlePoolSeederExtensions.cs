using Sudoku.Functions.Services;

namespace UnitTests.Mocks;

public static class MockPuzzlePoolSeederExtensions
{
    extension(Mock<IPuzzlePoolSeeder> mock)
    {
        public void SetupSeedPoolReturns(int seeded)
        {
            mock.Setup(x => x.SeedPoolAsync(It.IsAny<PuzzlePoolSeedFilter?>())).ReturnsAsync(seeded);
        }

        public void VerifySeedPoolCalledOnce()
        {
            mock.Verify(x => x.SeedPoolAsync(It.IsAny<PuzzlePoolSeedFilter?>()), Times.Once);
        }

        public void VerifySeedPoolCalledWithoutFilter()
        {
            mock.Verify(x => x.SeedPoolAsync(null), Times.Once);
        }

        public void VerifySeedPoolCalledWith(PuzzlePoolSeedFilter expected)
        {
            mock.Verify(x => x.SeedPoolAsync(expected), Times.Once);
        }

        public void VerifySeedPoolNeverCalled()
        {
            mock.Verify(x => x.SeedPoolAsync(It.IsAny<PuzzlePoolSeedFilter?>()), Times.Never);
        }
    }
}
