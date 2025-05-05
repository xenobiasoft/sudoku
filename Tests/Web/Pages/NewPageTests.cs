using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Pages;
using Sudoku.Web.Server.Services;
using UnitTests.Helpers;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku;

namespace UnitTests.Web.Pages;

public class NewPageTests : TestContext
{
    private readonly Mock<ISudokuGame> _mockSudokuGame;
    private readonly Mock<IGameStateManager>? _mockGameStorageManager;

    public NewPageTests()
    {
        _mockSudokuGame = new Mock<ISudokuGame>();
        _mockGameStorageManager = new Mock<IGameStateManager>();

        _mockSudokuGame.SetNewAsync(PuzzleFactory.GetPuzzle(Level.Easy));

        Services.AddSingleton(_mockGameStorageManager.Object);
        Services.AddSingleton(_mockSudokuGame.Object);
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
        _mockGameStorageManager!.VerifySaveAsyncCalled(Times.Once);
    }

    [Fact]
    public void OnInitializedAsync_NavigatesToGamePageWithPuzzleId()
    {
        // Arrange
        var navMan = Services.GetRequiredService<FakeNavigationManager>();
        _mockSudokuGame.SetLoadAsync(PuzzleFactory.GetPuzzle(Level.Easy));

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