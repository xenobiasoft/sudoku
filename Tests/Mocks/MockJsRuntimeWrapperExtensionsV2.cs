using Sudoku.Web.Server.Models;
using Sudoku.Web.Server.Services.Abstractions;
using System.Text.Json;

namespace UnitTests.Mocks;

public static class MockJsRuntimeWrapperExtensionsV2
{
    public static Mock<IJsRuntimeWrapper> SetupAliasV2(this Mock<IJsRuntimeWrapper> mock, string? alias)
    {
        mock
            .Setup(x => x.GetAsync(It.Is<string>(s => s == "sudoku-alias")))
            .ReturnsAsync(alias ?? string.Empty);

        return mock;
    }

    public static Mock<IJsRuntimeWrapper> SetupSavedGamesV2(this Mock<IJsRuntimeWrapper> mock, List<GameModel> savedGames)
    {
        var json = JsonSerializer.Serialize(savedGames);
        mock
            .Setup(x => x.GetAsync(It.Is<string>(s => s == "savedGames")))
            .ReturnsAsync(json);

        return mock;
    }

    public static Mock<IJsRuntimeWrapper> SetupEmptyStorage(this Mock<IJsRuntimeWrapper> mock)
    {
        mock
            .Setup(x => x.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(string.Empty);

        return mock;
    }

    public static Mock<IJsRuntimeWrapper> VerifyGetsAliasV2(this Mock<IJsRuntimeWrapper> mock, Func<Times> times)
    {
        mock.Verify(x => x.GetAsync(It.Is<string>(s => s == "sudoku-alias")), times);

        return mock;
    }

    public static Mock<IJsRuntimeWrapper> VerifyLoadsSavedGamesV2(this Mock<IJsRuntimeWrapper> mock, Func<Times> times)
    {
        mock.Verify(x => x.GetAsync(It.Is<string>(s => s == "savedGames")), times);

        return mock;
    }

    public static Mock<IJsRuntimeWrapper> VerifySavesAliasV2(this Mock<IJsRuntimeWrapper> mock, string alias, Func<Times> times)
    {
        mock.Verify(x => x.SetAsync("sudoku-alias", alias), times);

        return mock;
    }

    public static Mock<IJsRuntimeWrapper> VerifySavesGameV2(this Mock<IJsRuntimeWrapper> mock, Func<Times> times)
    {
        mock.Verify(x => x.SetAsync(It.Is<string>(s => s == "savedGames"), It.IsAny<string>()), times);

        return mock;
    }

    public static Mock<IJsRuntimeWrapper> VerifySavesGameV2(this Mock<IJsRuntimeWrapper> mock, string serializedGames, Func<Times> times)
    {
        mock.Verify(x => x.SetAsync(It.Is<string>(s => s == "savedGames"), serializedGames), times);

        return mock;
    }
}