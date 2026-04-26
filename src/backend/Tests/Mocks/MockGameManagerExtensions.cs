using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.Abstractions;

namespace UnitTests.Mocks;

public static class MockGameManagerExtensions
{
    extension(Mock<IGameManager> mock)
    {
        public void SetupCreateGameAsync(string alias, string difficulty)
        {
            var game = new GameModel
            {
                Id = Guid.NewGuid().ToString(),
                PlayerAlias = alias,
                Difficulty = difficulty,
                Status = "New",
                Statistics = new(),
                CreatedAt = DateTime.UtcNow,
                StartedAt = null,
                CompletedAt = null,
                PausedAt = null,
                Cells = []
            };
            mock
                .Setup(x => x.CreateGameAsync(alias, difficulty))
                .ReturnsAsync(game);
        }

        public void SetupLoadGameAsync(GameModel game)
        {
            mock
                .Setup(x => x.LoadGameAsync(game.PlayerAlias, It.IsAny<string>()))
                .ReturnsAsync(game);
        }

        public void SetupLoadGamesAsync(IEnumerable<GameModel> gameStates)
        {
            mock
                .Setup(x => x.LoadGamesAsync(It.IsAny<string>()))
                .ReturnsAsync(gameStates.ToList());
        }

        public void VerifyCreateGameAsyncCalled(string alias, string difficulty, Func<Times> times)
        {
            mock.Verify(x => x.CreateGameAsync(alias, difficulty), times);
        }

        public void VerifyDeleteGameAsyncCalled(Func<Times> times)
        {
            mock.Verify(x => x.DeleteGameAsync(It.IsAny<string>(), It.IsAny<string>()), times);
        }

        public void VerifyDeleteGameAsyncCalled(string alias, string puzzleId, Func<Times> times)
        {
            mock.Verify(x => x.DeleteGameAsync(alias, puzzleId), times);
        }

        public void VerifyLoadsAsyncCalled(string alias, string puzzleId, Func<Times> times)
        {
            mock.Verify(x => x.LoadGameAsync(alias, puzzleId), times);
        }

        public void VerifyMoveRecorded(int row, int column, int? value, Func<Times> times)
        {
            mock.Verify(x => x.RecordMove(row, column, value, It.IsAny<bool>()), times);
        }

        public void VerifyResetAsyncCalled(Func<Times> times)
        {
            mock.Verify(x => x.ResetGameAsync(), times);
        }

        public void VerifySaveAsyncCalled(Func<Times> times)
        {
            mock.Verify(x => x.SaveGameAsync(), times);
        }

        public void VerifyStartsGameAsync(Func<Times> times)
        {
            mock.Verify(x => x.StartGameAsync(), times);
        }

        public void VerifyUndoAsyncCalled(Func<Times> times)
        {
            mock.Verify(x => x.UndoGameAsync(), times);
        }
    }
}