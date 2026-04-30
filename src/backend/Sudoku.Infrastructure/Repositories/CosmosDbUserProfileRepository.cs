using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sudoku.Application.Interfaces;
using Sudoku.Domain.Entities;
using Sudoku.Domain.ValueObjects;
using Sudoku.Infrastructure.Configuration;
using Sudoku.Infrastructure.Mappers;
using Sudoku.Infrastructure.Models;

namespace Sudoku.Infrastructure.Repositories;

public class CosmosDbUserProfileRepository(
    CosmosClient cosmosClient,
    IOptions<CosmosDbOptions> options,
    ILogger<CosmosDbUserProfileRepository> logger) : IUserProfileRepository
{
    private const string ProfilesContainerName = "profiles";
    private Container? _container;

    public async Task<UserProfile?> GetByAliasAsync(PlayerAlias alias)
    {
        await EnsureContainerAsync();
        try
        {
            var sqlQuery = "SELECT * FROM c WHERE c.alias = @alias";
            var queryDefinition = new QueryDefinition(sqlQuery).WithParameter("@alias", alias.Value);
            var results = new List<UserProfileDocument>();
            using var iterator = _container!.GetItemQueryIterator<UserProfileDocument>(queryDefinition);
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }
            var document = results.FirstOrDefault();
            return document == null ? null : UserProfileMapper.ToDomain(document);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving profile for alias {Alias}", alias.Value);
            throw;
        }
    }

    public async Task<UserProfile?> GetByIdAsync(ProfileId id)
    {
        await EnsureContainerAsync();
        try
        {
            var idStr = id.ToString();
            var response = await _container!.ReadItemAsync<UserProfileDocument>(idStr, new PartitionKey(idStr));
            return UserProfileMapper.ToDomain(response.Resource);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving profile {ProfileId}", id);
            throw;
        }
    }

    public async Task<bool> AliasExistsAsync(PlayerAlias alias)
    {
        await EnsureContainerAsync();
        try
        {
            var sqlQuery = "SELECT VALUE COUNT(1) FROM c WHERE c.alias = @alias";
            var queryDefinition = new QueryDefinition(sqlQuery).WithParameter("@alias", alias.Value);
            var results = new List<int>();
            using var iterator = _container!.GetItemQueryIterator<int>(queryDefinition);
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }
            return results.FirstOrDefault() > 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking alias existence for {Alias}", alias.Value);
            throw;
        }
    }

    public async Task SaveAsync(UserProfile profile)
    {
        await EnsureContainerAsync();
        try
        {
            var document = UserProfileMapper.ToDocument(profile);
            await _container!.UpsertItemAsync(document, new PartitionKey(document.ProfileId));
            logger.LogDebug("Saved profile {ProfileId}", profile.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving profile {ProfileId}", profile.Id);
            throw;
        }
    }

    private async Task EnsureContainerAsync()
    {
        if (_container != null) return;
        var database = cosmosClient.GetDatabase(options.Value.DatabaseName);
        _container = database.GetContainer(ProfilesContainerName);
        await _container.ReadContainerAsync();
    }
}
