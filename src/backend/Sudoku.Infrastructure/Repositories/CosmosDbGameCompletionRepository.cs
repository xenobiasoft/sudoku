using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sudoku.Application.Interfaces;
using Sudoku.Application.Models;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Configuration;
using Sudoku.Infrastructure.Models;

namespace Sudoku.Infrastructure.Repositories;

public class CosmosDbGameCompletionRepository(
    CosmosClient cosmosClient,
    IOptions<CosmosDbOptions> options,
    ILogger<CosmosDbGameCompletionRepository> logger) : IGameCompletionRepository
{
    private const string CompletionsContainerName = "game-completions";
    private Container? _container;

    public async Task<IEnumerable<GameCompletion>> GetByProfileIdAsync(ProfileId profileId)
    {
        try
        {
            await EnsureContainerAsync();

            var sqlQuery = "SELECT * FROM c WHERE c.profileId = @profileId ORDER BY c.completedAt DESC";
            var queryDefinition = new QueryDefinition(sqlQuery).WithParameter("@profileId", profileId.ToString());
            var documents = new List<GameCompletionDocument>();

            // A player's completions all live in their own partition, so scope the query to it
            // rather than letting Cosmos fan out cross-partition.
            var queryOptions = new QueryRequestOptions { PartitionKey = new PartitionKey(profileId.ToString()) };

            using var iterator = _container!.GetItemQueryIterator<GameCompletionDocument>(queryDefinition, requestOptions: queryOptions);
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                documents.AddRange(response);
            }

            return documents.Select(ToDomain).ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving game completions for profile {ProfileId}", profileId);
            throw;
        }
    }

    public async Task<GameCompletion?> GetByGameIdAsync(GameId gameId, ProfileId profileId)
    {
        try
        {
            await EnsureContainerAsync();

            var response = await _container!.ReadItemAsync<GameCompletionDocument>(
                gameId.ToString(), new PartitionKey(profileId.ToString()));

            return ToDomain(response.Resource);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving game completion {GameId}", gameId);
            throw;
        }
    }

    public async Task UpsertAsync(GameCompletion completion)
    {
        try
        {
            await EnsureContainerAsync();

            var document = ToDocument(completion);

            await _container!.UpsertItemAsync(document, new PartitionKey(document.ProfileId));

            logger.LogDebug("Upserted game completion {GameId}", completion.GameId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error upserting game completion {GameId}", completion.GameId);
            throw;
        }
    }

    private static GameCompletionDocument ToDocument(GameCompletion completion) => new()
    {
        Id = completion.GameId,
        GameId = completion.GameId,
        ProfileId = completion.ProfileId,
        Difficulty = completion.Difficulty,
        GridSize = completion.GridSize,
        PlayDuration = completion.PlayDuration,
        CompletedAt = completion.CompletedAt
    };

    private static GameCompletion ToDomain(GameCompletionDocument document) => new(
        document.GameId,
        document.ProfileId,
        document.Difficulty,
        document.PlayDuration,
        document.CompletedAt,
        document.GridSize);

    private async Task EnsureContainerAsync()
    {
        if (_container != null) return;

        if (options.Value.AutoCreateContainers)
        {
            var dbResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(options.Value.DatabaseName);
            var containerProperties = new ContainerProperties(CompletionsContainerName, "/profileId");
            var containerResponse = await dbResponse.Database.CreateContainerIfNotExistsAsync(containerProperties);
            _container = containerResponse.Container;
        }
        else
        {
            var database = cosmosClient.GetDatabase(options.Value.DatabaseName);
            _container = database.GetContainer(CompletionsContainerName);
            await _container.ReadContainerAsync();
        }
    }
}
