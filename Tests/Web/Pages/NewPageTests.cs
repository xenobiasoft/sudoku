using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Pages;
using Sudoku.Web.Server.Services.Abstractions;
using UnitTests.Helpers;
using XenobiaSoft.Sudoku;

namespace UnitTests.Web.Pages;

public class NewPageTests : TestContext
{
    private readonly Mock<ISudokuGame> _mockSudokuGame;
    private readonly Mock<IGameManager>? _mockGameManager;

    public NewPageTests()
    {
        var alias = "game-alias";
        _mockSudokuGame = new Mock<ISudokuGame>();
        _mockGameManager = new Mock<IGameManager>();
        var aliasService = new Mock<IAliasService>();
        aliasService.SetupGetAliasAsync(alias);

        _mockSudokuGame.SetNewAsync(alias, PuzzleFactory.GetPuzzle(GameDifficulty.Easy));

        Services.AddSingleton(_mockGameManager.Object);
        Services.AddSingleton(_mockSudokuGame.Object);
        Services.AddSingleton(aliasService.Object);
    }

    [Fact]
    public void OnInitializedAsync_GeneratesNewPuzzle()
    {
        // Arrange

        // Act
        RenderComponent<New>(parameters => parameters.Add(p => p.Difficulty, "Medium"));

        // Assert
        _mockSudokuGame.VerifyGeneratesNewPuzzle(Times.Once);
    }

    [Fact]
    public void OnInitializedAsync_SavesGameState()
    {
        // Arrange

        // Act
        RenderComponent<New>(parameters => parameters.Add(p => p.Difficulty, "Medium"));

        // Assert
        _mockGameManager!.VerifySaveAsyncCalled(Times.Once);
    }

    [Fact]
    public void OnInitializedAsync_NavigatesToGamePageWithPuzzleId()
    {
        // Arrange
        var navMan = Services.GetRequiredService<FakeNavigationManager>();
        _mockSudokuGame.SetLoadAsync(PuzzleFactory.GetPuzzle(GameDifficulty.Easy));

        // Act
        RenderComponent<New>(parameters => parameters.Add(p => p.Difficulty, "Medium"));

        // Assert
        navMan.Uri.Should().StartWith("http://localhost/game/");
    }

    [Fact]
    public void RendersLoader()
    {
        // Arrange

        // Act
        var sut = RenderComponent<New>(parameters => parameters.Add(p => p.Difficulty, "Medium"));

        // Assert
        var loader = sut.Find("div.sudoku-loader");
        loader.Should().NotBeNull();
    }
}