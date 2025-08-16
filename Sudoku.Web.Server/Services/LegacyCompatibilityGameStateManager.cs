using Sudoku.Web.Server.Services.Abstractions;
using Sudoku.Web.Server.Services.Converters;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Web.Server.Services;

/// <summary>
/// Legacy compatibility service that implements IGameStateManager using the new API
/// </summary>
public class LegacyCompatibilityGameStateManager : IGameStateManager
{
    private readonly IApiBasedGameStateManager _apiGameStateManager;
    private readonly IAliasService _aliasService;
    private readonly ILogger<LegacyCompatibilityGameStateManager> _logger;

    public LegacyCompatibilityGameStateManager(
        IApiBasedGameStateManager apiGameStateManager,
        IAliasService aliasService,
        ILogger<LegacyCompatibilityGameStateManager> logger)
    {
        _apiGameStateManager = apiGameStateManager ?? throw new ArgumentNullException(nameof(apiGameStateManager));
        _aliasService = aliasService ?? throw new ArgumentNullException(nameof(aliasService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task DeleteGameAsync(string alias, string gameId)
    {
        _logger.LogInformation("Deleting game {GameId} for player {Alias}", gameId, alias);
        
        var result = await _apiGameStateManager.DeleteGameAsync(alias, gameId);
        
        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to delete game {GameId}: {Error}", gameId, result.Error);
            throw new InvalidOperationException($"Failed to delete game: {result.Error}");
        }
    }

    public async Task<GameStateMemory?> LoadGameAsync(string alias, string gameId)
    {
        _logger.LogInformation("Loading game {GameId} for player {Alias}", gameId, alias);
        
        var result = await _apiGameStateManager.LoadGameAsync(alias, gameId);
        
        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to load game {GameId}: {Error}", gameId, result.Error);
            return null;
        }

        if (result.Value == null)
        {
            _logger.LogWarning("Game {GameId} not found", gameId);
            return null;
        }

        return ModelConverter.ToGameStateMemory(result.Value);
    }

    public async Task<List<GameStateMemory>> LoadGamesAsync()
    {
        _logger.LogInformation("Loading all games for current user");
        
        try
        {
            var alias = await _aliasService.GetAliasAsync();
            var result = await _apiGameStateManager.LoadGamesAsync(alias);
            
            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to load games: {Error}", result.Error);
                return new List<GameStateMemory>();
            }

            if (result.Value == null)
            {
                _logger.LogInformation("No games found for player {Alias}", alias);
                return new List<GameStateMemory>();
            }

            return ModelConverter.ToGameStateMemoryList(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while loading games");
            return new List<GameStateMemory>();
        }
    }

    public async Task<GameStateMemory> ResetGameAsync(string alias, string gameId)
    {
        _logger.LogInformation("Resetting game {GameId} for player {Alias}", gameId, alias);
        
        var result = await _apiGameStateManager.ResetGameAsync(alias, gameId);
        
        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to reset game {GameId}: {Error}", gameId, result.Error);
            throw new InvalidOperationException($"Failed to reset game: {result.Error}");
        }

        // Load the updated game state
        var gameResult = await _apiGameStateManager.LoadGameAsync(alias, gameId);
        if (!gameResult.IsSuccess || gameResult.Value == null)
        {
            _logger.LogError("Failed to load game after reset {GameId}: {Error}", gameId, gameResult.Error);
            throw new InvalidOperationException($"Failed to load game after reset: {gameResult.Error}");
        }

        return ModelConverter.ToGameStateMemory(gameResult.Value);
    }

    public async Task SaveGameAsync(GameStateMemory gameState)
    {
        // This method is not needed anymore since the API handles persistence automatically
        // but we keep it for compatibility with existing code
        _logger.LogInformation("SaveGameAsync called - no action needed as API handles persistence automatically");
        await Task.CompletedTask;
    }

    public async Task<GameStateMemory> UndoGameAsync(string alias, string gameId)
    {
        _logger.LogInformation("Undoing move for game {GameId} for player {Alias}", gameId, alias);
        
        var result = await _apiGameStateManager.UndoGameAsync(alias, gameId);
        
        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to undo move for game {GameId}: {Error}", gameId, result.Error);
            throw new InvalidOperationException($"Failed to undo move: {result.Error}");
        }

        // Load the updated game state
        var gameResult = await _apiGameStateManager.LoadGameAsync(alias, gameId);
        if (!gameResult.IsSuccess || gameResult.Value == null)
        {
            _logger.LogError("Failed to load game after undo {GameId}: {Error}", gameId, gameResult.Error);
            throw new InvalidOperationException($"Failed to load game after undo: {gameResult.Error}");
        }

        return ModelConverter.ToGameStateMemory(gameResult.Value);
    }
}