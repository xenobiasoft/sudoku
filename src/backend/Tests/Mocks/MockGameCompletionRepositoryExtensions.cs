using Sudoku.Application.Interfaces;
using Sudoku.Application.Models;
using Sudoku.Domain.ValueObjects;

namespace UnitTests.Mocks;

public static class MockGameCompletionRepositoryExtensions
{
    extension(Mock<IGameCompletionRepository> mock)
    {
        public void SetupGetByProfileId(IEnumerable<GameCompletion> completions)
        {
            mock
                .Setup(x => x.GetByProfileIdAsync(It.IsAny<ProfileId>()))
                .ReturnsAsync(completions.ToList());
        }

        public void SetupGetByProfileIdThrows(Exception ex)
        {
            mock
                .Setup(x => x.GetByProfileIdAsync(It.IsAny<ProfileId>()))
                .ThrowsAsync(ex);
        }

        public void SetupCompletionExists(GameCompletion completion)
        {
            mock
                .Setup(x => x.GetByGameIdAsync(It.IsAny<GameId>(), It.IsAny<ProfileId>()))
                .ReturnsAsync(completion);
        }

        public void SetupCompletionNotFound()
        {
            mock
                .Setup(x => x.GetByGameIdAsync(It.IsAny<GameId>(), It.IsAny<ProfileId>()))
                .ReturnsAsync((GameCompletion?)null);
        }

        public void SetupUpsertThrows(Exception ex)
        {
            mock
                .Setup(x => x.UpsertAsync(It.IsAny<GameCompletion>()))
                .ThrowsAsync(ex);
        }

        public void VerifyUpserted(GameCompletion expected, Func<Times> times)
        {
            mock.Verify(x => x.UpsertAsync(It.Is<GameCompletion>(completion =>
                completion.GameId == expected.GameId &&
                completion.ProfileId == expected.ProfileId &&
                completion.Difficulty == expected.Difficulty &&
                completion.PlayDuration == expected.PlayDuration &&
                completion.CompletedAt == expected.CompletedAt)), times);
        }

        public void VerifyUpsertedGame(string gameId, Func<Times> times)
        {
            mock.Verify(x => x.UpsertAsync(It.Is<GameCompletion>(completion => completion.GameId == gameId)), times);
        }

        public void VerifyNeverUpserted()
        {
            mock.Verify(x => x.UpsertAsync(It.IsAny<GameCompletion>()), Times.Never);
        }
    }
}
