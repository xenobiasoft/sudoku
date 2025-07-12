# Refactoring Examples: Current Code to Clean Architecture

## Example 1: Refactoring SudokuGame to Domain Entity

### Current Implementation (Sudoku/SudokuGame.cs)

```csharp
public class SudokuGame(IPuzzleGenerator puzzleGenerator, IGameStateStorage gameStateStorage) : ISudokuGame
{
    public async Task<GameStateMemory> NewGameAsync(string alias, GameDifficulty difficulty)
    {
        var puzzle = await puzzleGenerator.GenerateAsync(difficulty).ConfigureAwait(false);
        var gameState = new GameStateMemory { /* ... */ };
        return gameState;
    }

    public Task SaveAsync(GameStateMemory memory) => gameStateStorage.SaveAsync(memory);
    public Task DeleteAsync(string alias, string puzzleId) => gameStateStorage.DeleteAsync(alias, puzzleId);
    public Task<GameStateMemory> LoadAsync(string alias, string puzzleId) => gameStateStorage.LoadAsync(alias, puzzleId);
}
```

### Proposed Domain Entity (Sudoku.Domain/Entities/SudokuGame.cs)

```csharp
public class SudokuGame : AggregateRoot
{
    private readonly List<Cell> _cells;
    private readonly List<DomainEvent> _domainEvents;

    public GameId Id { get; private set; }
    public PlayerAlias PlayerAlias { get; private set; }
    public GameDifficulty Difficulty { get; private set; }
    public GameStatus Status { get; private set; }
    public GameStatistics Statistics { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private SudokuGame() { } // For EF Core

    public static SudokuGame Create(PlayerAlias playerAlias, GameDifficulty difficulty, SudokuPuzzle puzzle)
    {
        var game = new SudokuGame
        {
            Id = GameId.New(),
            PlayerAlias = playerAlias,
            Difficulty = difficulty,
            Status = GameStatus.InProgress,
            Statistics = GameStatistics.Create(),
            CreatedAt = DateTime.UtcNow,
            _cells = puzzle.GetAllCells().ToList()
        };

        game.AddDomainEvent(new GameCreatedEvent(game.Id, playerAlias, difficulty));
        return game;
    }

    public void MakeMove(int row, int column, int value)
    {
        if (Status != GameStatus.InProgress)
            throw new GameNotInProgressException(Id);

        if (!IsValidMove(row, column, value))
            throw new InvalidMoveException(Id, row, column, value);

        var cell = GetCell(row, column);
        if (cell.IsFixed)
            throw new CellIsFixedException(Id, row, column);

        cell.SetValue(value);
        Statistics.RecordMove(IsValidMove(row, column, value));

        AddDomainEvent(new MoveMadeEvent(Id, row, column, value, Statistics));

        if (IsGameComplete())
        {
            CompleteGame();
        }
    }

    public void PauseGame()
    {
        if (Status != GameStatus.InProgress)
            throw new GameNotInProgressException(Id);

        Status = GameStatus.Paused;
        AddDomainEvent(new GamePausedEvent(Id));
    }

    public void ResumeGame()
    {
        if (Status != GameStatus.Paused)
            throw new GameNotPausedException(Id);

        Status = GameStatus.InProgress;
        AddDomainEvent(new GameResumedEvent(Id));
    }

    private void CompleteGame()
    {
        Status = GameStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        AddDomainEvent(new GameCompletedEvent(Id, Statistics));
    }

    private bool IsValidMove(int row, int column, int value)
    {
        // Business logic for move validation
        return true; // Simplified for example
    }

    private bool IsGameComplete()
    {
        return _cells.All(c => c.HasValue) && _cells.All(c => IsValidMove(c.Row, c.Column, c.Value!.Value));
    }

    public IReadOnlyCollection<DomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();
    public void ClearDomainEvents() => _domainEvents.Clear();

    private void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
```

## Example 2: Application Service Implementation

### Current GameService (Sudoku/Abstractions/IGameService.cs)

```csharp
public interface IGameService
{
    Task<string> CreateGameAsync(string alias, GameDifficulty difficulty);
    Task DeleteGameAsync(string alias, string gameId);
    Task<GameStateMemory?> LoadGameAsync(string alias, string gameId);
    Task<IEnumerable<GameStateMemory>> LoadGamesAsync(string alias);
    Task<GameStateMemory> ResetGameAsync(string alias, string gameId);
    Task SaveGameAsync(GameStateMemory gameState);
    Task<GameStateMemory> UndoGameAsync(string alias, string gameId);
}
```

### Proposed Application Service (Sudoku.Application/Services/GameApplicationService.cs)

```csharp
public class GameApplicationService : IGameApplicationService
{
    private readonly IGameRepository _gameRepository;
    private readonly IPuzzleGenerator _puzzleGenerator;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<GameApplicationService> _logger;

    public GameApplicationService(
        IGameRepository gameRepository,
        IPuzzleGenerator puzzleGenerator,
        IDomainEventDispatcher eventDispatcher,
        ILogger<GameApplicationService> logger)
    {
        _gameRepository = gameRepository;
        _puzzleGenerator = puzzleGenerator;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
    }

    public async Task<Result<GameDto>> CreateGameAsync(CreateGameCommand command)
    {
        try
        {
            var playerAlias = PlayerAlias.Create(command.PlayerAlias);
            var puzzle = await _puzzleGenerator.GenerateAsync(command.Difficulty);

            var game = SudokuGame.Create(playerAlias, command.Difficulty, puzzle);
            await _gameRepository.SaveAsync(game);

            // Dispatch domain events
            await DispatchDomainEvents(game);

            _logger.LogInformation("Created new game {GameId} for player {PlayerAlias}",
                game.Id, playerAlias);

            return Result<GameDto>.Success(GameDto.FromGame(game));
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain exception while creating game for player {PlayerAlias}",
                command.PlayerAlias);
            return Result<GameDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating game for player {PlayerAlias}",
                command.PlayerAlias);
            return Result<GameDto>.Failure("An unexpected error occurred");
        }
    }

    public async Task<Result<GameDto>> MakeMoveAsync(MakeMoveCommand command)
    {
        try
        {
            var game = await _gameRepository.GetByIdAsync(command.GameId);
            if (game == null)
                return Result<GameDto>.Failure("Game not found");

            game.MakeMove(command.Row, command.Column, command.Value);
            await _gameRepository.SaveAsync(game);

            // Dispatch domain events
            await DispatchDomainEvents(game);

            return Result<GameDto>.Success(GameDto.FromGame(game));
        }
        catch (DomainException ex)
        {
            return Result<GameDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<IEnumerable<GameDto>>> GetPlayerGamesAsync(GetPlayerGamesQuery query)
    {
        try
        {
            var playerAlias = PlayerAlias.Create(query.PlayerAlias);
            var games = await _gameRepository.GetByPlayerAsync(playerAlias);

            var gameDtos = games.Select(GameDto.FromGame);
            return Result<IEnumerable<GameDto>>.Success(gameDtos);
        }
        catch (DomainException ex)
        {
            return Result<IEnumerable<GameDto>>.Failure(ex.Message);
        }
    }

    private async Task DispatchDomainEvents(SudokuGame game)
    {
        var events = game.GetDomainEvents();
        foreach (var domainEvent in events)
        {
            await _eventDispatcher.DispatchAsync(domainEvent);
        }
        game.ClearDomainEvents();
    }
}
```

## Example 3: Repository Implementation

### Current Storage (Sudoku.Storage.Azure)

```csharp
public class AzureBlobGameStateStorage : IGameStateStorage
{
    public async Task SaveAsync(GameStateMemory gameState)
    {
        // Direct storage implementation
    }
}
```

### Proposed Repository (Sudoku.Infrastructure/Repositories/AzureBlobGameRepository.cs)

```csharp
public class AzureBlobGameRepository : IGameRepository
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<AzureBlobGameRepository> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public AzureBlobGameRepository(
        BlobServiceClient blobServiceClient,
        ILogger<AzureBlobGameRepository> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<SudokuGame?> GetByIdAsync(GameId id)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("games");
            var blobClient = containerClient.GetBlobClient($"{id.Value}.json");

            if (!await blobClient.ExistsAsync())
                return null;

            var response = await blobClient.DownloadAsync();
            using var streamReader = new StreamReader(response.Value.Content);
            var json = await streamReader.ReadToEndAsync();

            return JsonSerializer.Deserialize<SudokuGame>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving game {GameId}", id);
            throw new RepositoryException($"Failed to retrieve game {id}", ex);
        }
    }

    public async Task<IEnumerable<SudokuGame>> GetByPlayerAsync(PlayerAlias playerAlias)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("games");
            var games = new List<SudokuGame>();

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                var response = await blobClient.DownloadAsync();
                using var streamReader = new StreamReader(response.Value.Content);
                var json = await streamReader.ReadToEndAsync();

                var game = JsonSerializer.Deserialize<SudokuGame>(json, _jsonOptions);
                if (game?.PlayerAlias == playerAlias)
                {
                    games.Add(game);
                }
            }

            return games;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving games for player {PlayerAlias}", playerAlias);
            throw new RepositoryException($"Failed to retrieve games for player {playerAlias}", ex);
        }
    }

    public async Task SaveAsync(SudokuGame game)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("games");
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient($"{game.Id.Value}.json");
            var json = JsonSerializer.Serialize(game, _jsonOptions);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await blobClient.UploadAsync(stream, overwrite: true);

            _logger.LogDebug("Saved game {GameId} to Azure Blob Storage", game.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving game {GameId}", game.Id);
            throw new RepositoryException($"Failed to save game {game.Id}", ex);
        }
    }

    public async Task DeleteAsync(GameId id)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("games");
            var blobClient = containerClient.GetBlobClient($"{id.Value}.json");

            await blobClient.DeleteIfExistsAsync();
            _logger.LogDebug("Deleted game {GameId} from Azure Blob Storage", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting game {GameId}", id);
            throw new RepositoryException($"Failed to delete game {id}", ex);
        }
    }
}
```

## Example 4: API Controller Refactoring

### Current Controller (Sudoku.Api/Controllers/GamesController.cs)

```csharp
[Route("api/players/{alias}/[controller]")]
[ApiController]
public class GamesController(IGameService gameService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> CreateAsync(string alias, [FromQuery] GameDifficulty difficulty)
    {
        if (string.IsNullOrWhiteSpace(alias))
            return BadRequest("Player alias cannot be null or empty.");

        var gameId = await gameService.CreateGameAsync(alias, difficulty);
        return CreatedAtAction(nameof(GetAsync), new { alias, gameId }, gameId);
    }
}
```

### Proposed Controller (Sudoku.Api/Controllers/GamesController.cs)

```csharp
[Route("api/players/{alias}/[controller]")]
[ApiController]
public class GamesController(IGameApplicationService gameApplicationService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GameDto>> CreateAsync(
        string alias,
        [FromQuery] GameDifficulty difficulty)
    {
        var command = new CreateGameCommand(alias, difficulty);
        var result = await gameApplicationService.CreateGameAsync(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Failed to create game",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return CreatedAtAction(
            nameof(GetAsync),
            new { alias, gameId = result.Value.Id },
            result.Value);
    }

    [HttpPost("{gameId}/moves")]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameDto>> MakeMoveAsync(
        string alias,
        string gameId,
        [FromBody] MakeMoveRequest request)
    {
        var command = new MakeMoveCommand(
            GameId.Create(gameId),
            request.Row,
            request.Column,
            request.Value);

        var result = await gameApplicationService.MakeMoveAsync(command);

        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("not found") == true)
                return NotFound(new ProblemDetails
                {
                    Title = "Game not found",
                    Detail = result.Error,
                    Status = StatusCodes.Status404NotFound
                });

            return BadRequest(new ProblemDetails
            {
                Title = "Invalid move",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return Ok(result.Value);
    }
}
```

## Example 5: Value Objects

### Current Implementation

```csharp
public enum GameDifficulty { Easy, Medium, Hard }
public string Alias { get; set; }
public string PuzzleId { get; set; }
```

### Proposed Value Objects (Sudoku.Domain/ValueObjects/)

```csharp
public record GameDifficulty
{
    public static readonly GameDifficulty Easy = new(1, "Easy");
    public static readonly GameDifficulty Medium = new(2, "Medium");
    public static readonly GameDifficulty Hard = new(3, "Hard");

    public int Value { get; }
    public string Name { get; }

    private GameDifficulty(int value, string name)
    {
        Value = value;
        Name = name;
    }

    public static GameDifficulty FromValue(int value) => value switch
    {
        1 => Easy,
        2 => Medium,
        3 => Hard,
        _ => throw new InvalidGameDifficultyException(value)
    };
}

public record PlayerAlias
{
    public string Value { get; }

    private PlayerAlias(string value)
    {
        Value = value;
    }

    public static PlayerAlias Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidPlayerAliasException("Player alias cannot be null or empty");

        if (value.Length > 50)
            throw new InvalidPlayerAliasException("Player alias cannot exceed 50 characters");

        return new PlayerAlias(value.Trim());
    }
}

public record GameId
{
    public Guid Value { get; }

    private GameId(Guid value)
    {
        Value = value;
    }

    public static GameId New() => new(Guid.NewGuid());
    public static GameId Create(Guid value) => new(value);
    public static GameId Create(string value) => new(Guid.Parse(value));
}
```

These examples demonstrate how the current architecture can be refactored to follow Clean Architecture principles, providing better separation of concerns, improved testability, and more maintainable code.
