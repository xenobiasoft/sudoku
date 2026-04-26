using Sudoku.Blazor.Models;
using Sudoku.Blazor.Services.Abstractions;
using System.Text.Json;

namespace UnitTests.Mocks;

public static class MockJsRuntimeWrapperExtensionsV2
{
    extension(Mock<IJsRuntimeWrapper> mock)
    {
        public Mock<IJsRuntimeWrapper> SetupAliasV2(string? alias)
        {
            mock
                .Setup(x => x.GetAsync(It.Is<string>(s => s == "sudoku-alias")))
                .ReturnsAsync(alias ?? string.Empty);

            return mock;
        }

        public Mock<IJsRuntimeWrapper> SetupSavedGamesV2(List<GameModel> savedGames)
        {
            var json = JsonSerializer.Serialize(savedGames);
            mock
                .Setup(x => x.GetAsync(It.Is<string>(s => s == "savedGames")))
                .ReturnsAsync(json);

            return mock;
        }

        public Mock<IJsRuntimeWrapper> SetupEmptyStorage()
        {
            mock
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(string.Empty);

            return mock;
        }

        public Mock<IJsRuntimeWrapper> VerifyGetsAliasV2(Func<Times> times)
        {
            mock.Verify(x => x.GetAsync(It.Is<string>(s => s == "sudoku-alias")), times);

            return mock;
        }

        public Mock<IJsRuntimeWrapper> VerifyLoadsSavedGamesV2(Func<Times> times)
        {
            mock.Verify(x => x.GetAsync(It.Is<string>(s => s == "savedGames")), times);

            return mock;
        }

        public Mock<IJsRuntimeWrapper> VerifySavesAliasV2(string alias, Func<Times> times)
        {
            mock.Verify(x => x.SetAsync("sudoku-alias", alias), times);

            return mock;
        }

        public Mock<IJsRuntimeWrapper> VerifySavesGameV2(Func<Times> times)
        {
            mock.Verify(x => x.SetAsync(It.Is<string>(s => s == "savedGames"), It.IsAny<string>()), times);

            return mock;
        }

        public Mock<IJsRuntimeWrapper> VerifySavesGameV2(string serializedGames, Func<Times> times)
        {
            mock.Verify(x => x.SetAsync(It.Is<string>(s => s == "savedGames"), serializedGames), times);

            return mock;
        }
    }
}