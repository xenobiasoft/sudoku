# Error Handling Quick Reference Guide

## ?? When to Use Each Error Handling Approach

### Use Error Boundary (Preferred)
? User actions that can be retried
? Non-critical operations
? API calls that might fail temporarily
? Operations where user can continue

**Examples:**
- Loading saved games
- Deleting a game
- Making a move
- Resetting game
- Undoing a move

### Use Error Page Navigation
?? Critical initialization failures
?? Page cannot render at all
?? User must start over

**Examples:**
- Failed to authenticate user
- Page initialization completely failed
- Required services unavailable

### Use Logging Only
?? Optional features
?? Background operations
?? Non-blocking failures

**Examples:**
- Failed to save pencil marks
- Analytics tracking failed
- Non-critical cache updates

---

## ?? Implementation Checklist

### For New Pages

- [ ] Create dedicated ErrorBoundary component
- [ ] Wrap page content with ErrorBoundary in .razor file
- [ ] Remove try-catch from non-critical methods in .razor.cs
- [ ] Keep try-catch only for initialization in .razor.cs
- [ ] Navigate to `/error/500` for critical failures only
- [ ] Add comprehensive logging
- [ ] Test error scenarios

### For Existing Pages

- [ ] Analyze error handling approach
- [ ] Identify critical vs recoverable errors
- [ ] Create ErrorBoundary component if needed
- [ ] Refactor try-catch blocks appropriately
- [ ] Update navigation URLs to include status codes
- [ ] Verify logging is comprehensive
- [ ] Test error recovery

---

## ?? Error Boundary Template

```razor
@inherits ErrorBoundary

<div class="blazor-error-boundary">
    @if (CurrentException is not null)
    {
        <div class="alert alert-danger" role="alert">
            <h4 class="alert-heading">[Error Title]</h4>
            <p>[User-friendly message explaining what happened]</p>
            
            @if (ShowExceptionDetails)
            {
                <hr>
                <details>
                    <summary>Error Details (Development Mode)</summary>
                    <pre class="mt-2">@CurrentException.ToString()</pre>
                </details>
            }
            
            <div class="mt-3">
                <button class="btn btn-primary" @onclick="Recover">Try Again</button>
                <a href="/" class="btn btn-secondary">Back to Home</a>
            </div>
        </div>
    }
    else
    {
        @ChildContent
    }
</div>

@code {
    [Inject] private IWebHostEnvironment Environment { get; set; } = default!;
    [Inject] private ILogger<[ComponentName]> Logger { get; set; } = default!;

    private bool ShowExceptionDetails => Environment.IsDevelopment();

    protected override Task OnErrorAsync(Exception exception)
    {
        Logger.LogError(exception, "[Context message]");
        return base.OnErrorAsync(exception);
    }
}
```

---

## ?? Error Message Guidelines

### Good Error Messages ?
- "Unable to load saved games"
- "Failed to generate puzzle"
- "An error occurred while processing your move"
- "Could not connect to game server"

### Bad Error Messages ?
- "Error"
- "Something went wrong"
- "System.NullReferenceException"
- "500 Internal Server Error"

### Message Structure
1. **What happened**: Brief description
2. **Why it matters**: Impact on user
3. **What to do**: Clear action steps

**Example:**
```
? Unable to Load Game Menu

We encountered an error while loading your saved games. 
This might be a temporary issue.

[Try Again] [Reload Page]
```

---

## ?? Error Logging Best Practices

### Log Levels

```csharp
// Critical - Application cannot continue
Logger.LogCritical(ex, "Database connection failed");

// Error - Operation failed, but app continues
Logger.LogError(ex, "Failed to load game {GameId}", gameId);

// Warning - Something unusual, might be a problem
Logger.LogWarning(ex, "Failed to save pencil marks for cell");

// Information - Track flow of application
Logger.LogInformation("User {User} started new game", user);

// Debug - Detailed information for debugging
Logger.LogDebug("Cell value changed from {Old} to {New}", old, new);
```

### Include Context

```csharp
// ? Bad - No context
Logger.LogError(ex, "Error loading game");

// ? Good - With context
Logger.LogError(ex, 
    "Failed to load game {GameId} for player {Player}", 
    gameId, 
    playerAlias);
```

---

## ?? Testing Error Handling

### Manual Testing Checklist

#### Index Page
- [ ] Disconnect API, verify error boundary appears
- [ ] Click "Try Again" button
- [ ] Click "Reload Page" button
- [ ] Delete game when API is slow
- [ ] Verify exception details in dev mode

#### New Page
- [ ] Generate game with invalid difficulty
- [ ] Generate game with API down
- [ ] Click "Try Again" button
- [ ] Click "Back to Menu" button
- [ ] Verify exception details in dev mode

#### Game Page
- [ ] Load non-existent game
- [ ] Make move when API is down
- [ ] Reset game when API is down
- [ ] Undo when no moves exist
- [ ] Click "Try Again" button
- [ ] Verify exception details in dev mode

### Automated Testing

```csharp
[Fact]
public void ErrorBoundary_DisplaysError_WhenExceptionThrown()
{
    // Arrange
    var mockService = new Mock<IGameManager>();
    mockService.Setup(x => x.LoadGamesAsync(It.IsAny<string>()))
        .ThrowsAsync(new Exception("Test error"));
    
    // Act
    var component = RenderComponent<Index>();
    
    // Assert
    component.Find(".alert-danger").Should().NotBeNull();
    component.Find(".alert-heading").TextContent
        .Should().Contain("Unable to Load");
}
```

---

## ?? Troubleshooting

### Error Boundary Not Catching Errors

**Problem:** Exceptions bypass error boundary

**Solutions:**
1. ? Ensure exception is thrown from within component render
2. ? Verify ErrorBoundary wraps the content properly
3. ? Check if try-catch is catching before boundary sees it
4. ? Confirm exception happens in child content, not parent

### Status Code Not Showing

**Problem:** Error page shows wrong or no status code

**Solutions:**
1. ? Include status code in navigation: `/error/500`
2. ? Verify Error.cshtml route accepts parameter: `@page "/error/{statusCode?}"`
3. ? Check Error.cshtml.cs handles statusCode parameter

### Exception Details Not Showing

**Problem:** Stack trace not visible in development

**Solutions:**
1. ? Verify `Environment.IsDevelopment()` returns true
2. ? Check `ShowExceptionDetails` property
3. ? Ensure `CurrentException` is not null
4. ? Verify `<details>` element is rendered

---

## ?? Additional Resources

### Microsoft Documentation
- [Handle errors in ASP.NET Core Blazor apps](https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/handle-errors)
- [Error Boundaries in Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/handle-errors#error-boundaries)

### Error Boundary Benefits
- Prevents white screen of death
- Improves user experience
- Maintains application state
- Enables graceful recovery
- Provides debugging information

### When NOT to Use Error Boundaries
- Async event handlers (use try-catch)
- Component constructor errors
- Server-side rendering errors
- Static constructor errors

---

## ? Implementation Status

| Page | Error Boundary | Critical Handling | Status |
|------|---------------|-------------------|--------|
| Index | ? IndexErrorBoundary | ? `/error/500` | Complete |
| New | ? NewGameErrorBoundary | ? Error Boundary Only | Complete |
| Game | ? GameErrorBoundary | ? `/error/500` | Complete |

---

## ?? Key Takeaways

1. **Error Boundaries = User Experience** - Always prefer error boundaries for recoverable errors
2. **Navigation = Critical Failures** - Only navigate to error page when page cannot render
3. **Logging = Debugging** - Comprehensive logging helps troubleshoot production issues
4. **Context = Clarity** - Always include context in error messages and logs
5. **Testing = Confidence** - Test error scenarios to ensure graceful handling

---

*Last Updated: [Current Date]*
*Version: 1.0*
