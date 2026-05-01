---
paths:
  - "src/frontend/Sudoku.Blazor/**/*.razor"
  - "src/frontend/Sudoku.Blazor/**/*.cs"
---

# Blazor Component Guidelines

## Component Structure
```csharp
@page "/game/{GameId}"
@using Sudoku.Application
@inject IGameApplicationService GameService

<PageTitle>Sudoku Game</PageTitle>

<div class="sudoku-board">
    @if (game != null)
    {
        <SudokuBoard Game="@game" OnMoveMade="HandleMoveMade" />
    }
</div>

@code {
    [Parameter] public string GameId { get; set; } = string.Empty;
    private GameDto? game;

    protected override async Task OnInitializedAsync()
    {
        var result = await GameService.GetGameAsync(GameId);
        if (result.IsSuccess)
            game = result.Value;
    }

    private async Task HandleMoveMade(int row, int column, int value)
    {
        // Handle move logic
    }
}
```

## Rules
- Use `@inject` for dependency injection in components
- Handle `Result<T>` from application services — don't assume success
- Use `async/await` for all service calls
- Blazor components are tested with bunit
