using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Pages;
using Sudoku.Web.Server.Services;
using UnitTests.Helpers;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku;
using XenobiaSoft.Sudoku.GameState;

namespace UnitTests.Web.Pages;

public class NewPageTests : TestContext
{
    private readonly Mock<IGameStateManager> _mockGameStateManager;
    private readonly Mock<ILocalStorageService> _mockLocalStorageService;
    private readonly Mock<ISudokuGame> _mockSudokuGame;

    public NewPageTests()
    {
        _mockSudokuGame = new Mock<ISudokuGame>();
        _mockLocalStorageService = new Mock<ILocalStorageService>();
        _mockGameStateManager = new Mock<IGameStateManager>();

        _mockSudokuGame.SetNewAsync(PuzzleFactory.GetPuzzle(Level.Easy));

        Services.AddSingleton(_mockGameStateManager.Object);
        Services.AddSingleton(_mockLocalStorageService.Object);
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
    public void OnInitializedAsync_SavesGameStateToPersistentStorage()
    {
        // Arrange

        // Act
        RenderComponent<New>(parameters => parameters.Add(p => p.Difficulty, "Medium"));

        // Assert
        _mockGameStateManager.VerifySaveAsyncCalled(Times.Once);
    }

    [Fact]
    public void OnInitializedAsync_SavesGameStateToLocalStorage()
    {
        // Arrange

        // Act
        RenderComponent<New>(parameters => parameters.Add(p => p.Difficulty, "Medium"));

        // Assert
        _mockLocalStorageService.VerifySaveGameAsyncCalled(Times.Once);
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