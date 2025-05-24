using Sudoku.Web.Server.Services.Abstractions;
using System.Text.Json;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Helpers.Mocks;

public static class MockJsRuntimeExtensions
{
    public static Mock<IJsRuntimeWrapper> SetupAlias(this Mock<IJsRuntimeWrapper> mock, string alias)
    {
        mock
            .Setup(x => x.GetAsync(It.Is<string>(s => s == "sudoku-alias")))
            .ReturnsAsync(alias);

        return mock;
    }

    public static Mock<IJsRuntimeWrapper> SetupSavedGames(this Mock<IJsRuntimeWrapper> mock, List<GameStateMemory> savedGames)
    {
        var json = JsonSerializer.Serialize(savedGames);
        mock
            .Setup(x => x.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(json);

        return mock;
    }

    public static Mock<IJsRuntimeWrapper> VerifyGetsAlias(this Mock<IJsRuntimeWrapper> mock, Func<Times> times)
    {
        mock.Verify(x => x.GetAsync(It.Is<string>(s => s == "sudoku-alias")), times);

        return mock;
    }

    public static Mock<IJsRuntimeWrapper> VerifyLoadsSavedGames(this Mock<IJsRuntimeWrapper> mock, Func<Times> times)
    {
        mock.Verify(x => x.GetAsync(It.IsAny<string>()), times);

        return mock;
    }

    public static Mock<IJsRuntimeWrapper> VerifySavesAlias(this Mock<IJsRuntimeWrapper> mock, string alias, Func<Times> times)
    {
        mock.Verify(x => x.SetAsync("sudoku-alias", alias), times);

        return mock;
    }

    public static Mock<IJsRuntimeWrapper> VerifySavesGame(this Mock<IJsRuntimeWrapper> mock, Func<Times> times)
    {
        mock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>()), times);

        return mock;
    }

    public static Mock<IJsRuntimeWrapper> VerifySavesGame(this Mock<IJsRuntimeWrapper> mock, string serializedGames, Func<Times> times)
    {
        mock.Verify(x => x.SetAsync(It.IsAny<string>(), serializedGames), times);

        return mock;
    }
}