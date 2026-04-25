using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;

namespace Sudoku.McpServer.Tools;

/// <summary>
/// MCP tools for querying Application Insights / Log Analytics telemetry.
/// All queries run against the Log Analytics workspace that backs the
/// Application Insights instance, using the App Service's Managed Identity.
/// </summary>
[McpServerToolType]
public sealed class ApplicationInsightsTools
{
    private readonly LogsQueryClient _logsClient;
    private readonly string _workspaceId;

    public ApplicationInsightsTools(LogsQueryClient logsClient, IConfiguration config)
    {
        _logsClient = logsClient;

        var workspaceId = config["AppInsights:WorkspaceId"];
        if (string.IsNullOrWhiteSpace(workspaceId))
        {
            throw new InvalidOperationException(
                "AppInsights:WorkspaceId is required and cannot be empty. Set it in app settings (Log Analytics workspace GUID).");
        }

        _workspaceId = workspaceId;
    }

    // -------------------------------------------------------------------------
    // Raw KQL access
    // -------------------------------------------------------------------------

    [McpServerTool, Description(
        "Run an arbitrary KQL query against the Log Analytics workspace that backs Application Insights. " +
        "Returns results as a pipe-delimited table. Use this for ad-hoc investigation.")]
    public async Task<string> QueryLogs(
        [Description("KQL query to execute. Tables available: requests, exceptions, traces, customEvents, dependencies, pageViews, performanceCounters.")] string kql,
        [Description("Look-back window in hours (1–720). Defaults to 1.")] int hours = 1,
        CancellationToken cancellationToken = default)
    {
        hours = Math.Clamp(hours, 1, 720);
        var response = await _logsClient.QueryWorkspaceAsync(
            _workspaceId, kql,
            new QueryTimeRange(TimeSpan.FromHours(hours)),
            cancellationToken: cancellationToken);

        return FormatTables(response.Value);
    }

    // -------------------------------------------------------------------------
    // Exception summary
    // -------------------------------------------------------------------------

    [McpServerTool, Description(
        "Return the top N most frequent exceptions recorded in Application Insights, " +
        "ordered by count descending. Useful for identifying recurring errors.")]
    public async Task<string> GetExceptionSummary(
        [Description("Look-back window in hours. Defaults to 24.")] int hours = 24,
        [Description("Maximum number of exception types to return. Defaults to 10.")] int topN = 10,
        CancellationToken cancellationToken = default)
    {
        hours = Math.Clamp(hours, 1, 720);
        topN = Math.Clamp(topN, 1, 100);

        var kql = $"""
            exceptions
            | summarize Count = count(), SampleMessage = any(outerMessage) by type
            | top {topN} by Count desc
            | project ExceptionType = type, Count, SampleMessage
            """;

        return await ExecuteKql(kql, hours, cancellationToken);
    }

    // -------------------------------------------------------------------------
    // Request / availability metrics
    // -------------------------------------------------------------------------

    [McpServerTool, Description(
        "Return HTTP request metrics per operation: total requests, failure rate, and average duration. " +
        "Covers both the Blazor frontend and the REST API.")]
    public async Task<string> GetRequestMetrics(
        [Description("Look-back window in hours. Defaults to 1.")] int hours = 1,
        [Description("Filter to a specific cloud role name (e.g. 'XenobiasoftSudokuApi-prod'). Leave empty for all roles.")] string? roleName = null,
        CancellationToken cancellationToken = default)
    {
        hours = Math.Clamp(hours, 1, 720);

        var roleFilter = string.IsNullOrWhiteSpace(roleName)
            ? string.Empty
            : $"| where cloud_RoleName == '{roleName}'";

        var kql = $"""
            requests
            {roleFilter}
            | summarize
                TotalRequests = count(),
                FailedRequests = countif(success == false),
                AvgDurationMs = round(avg(duration), 1)
              by name, cloud_RoleName
            | extend FailureRate = round(todouble(FailedRequests) / TotalRequests * 100, 2)
            | project Operation = name, Role = cloud_RoleName, TotalRequests, FailedRequests, FailureRate, AvgDurationMs
            | order by TotalRequests desc
            """;

        return await ExecuteKql(kql, hours, cancellationToken);
    }

    // -------------------------------------------------------------------------
    // Game-specific telemetry
    // -------------------------------------------------------------------------

    [McpServerTool, Description(
        "Return counts of domain events emitted by the Sudoku game (GameCreated, MoveMade, GameCompleted, etc.) " +
        "bucketed by event name. Useful for understanding game activity volume.")]
    public async Task<string> GetGameTelemetry(
        [Description("Look-back window in hours. Defaults to 24.")] int hours = 24,
        [Description("Granularity bucket in minutes for time-series breakdown (e.g. 60 for hourly). Pass 0 to get totals only.")] int bucketMinutes = 0,
        CancellationToken cancellationToken = default)
    {
        hours = Math.Clamp(hours, 1, 720);

        string kql;

        if (bucketMinutes <= 0)
        {
            kql = """
                customEvents
                | summarize Count = count() by name
                | order by Count desc
                """;
        }
        else
        {
            bucketMinutes = Math.Max(bucketMinutes, 1);
            kql = $"""
                customEvents
                | summarize Count = count() by name, bin(timestamp, {bucketMinutes}m)
                | order by timestamp asc, Count desc
                """;
        }

        return await ExecuteKql(kql, hours, cancellationToken);
    }

    // -------------------------------------------------------------------------
    // Active users
    // -------------------------------------------------------------------------

    [McpServerTool, Description(
        "Return the count of unique active users (by session or user ID) over the specified window. " +
        "Breaks down by hour to show traffic patterns.")]
    public async Task<string> GetActiveUsers(
        [Description("Look-back window in hours (1–168). Defaults to 24.")] int hours = 24,
        CancellationToken cancellationToken = default)
    {
        hours = Math.Clamp(hours, 1, 168);

        var kql = """
            requests
            | where isnotempty(user_Id) or isnotempty(session_Id)
            | summarize
                UniqueUsers    = dcount(user_Id),
                UniqueSessions = dcount(session_Id),
                TotalRequests  = count()
              by bin(timestamp, 1h)
            | order by timestamp asc
            """;

        return await ExecuteKql(kql, hours, cancellationToken);
    }

    // -------------------------------------------------------------------------
    // Dependency health
    // -------------------------------------------------------------------------

    [McpServerTool, Description(
        "Return dependency call health (Cosmos DB, HTTP calls, etc.): total calls, failure count, " +
        "average duration. Highlights slow or failing downstream dependencies.")]
    public async Task<string> GetDependencyHealth(
        [Description("Look-back window in hours. Defaults to 1.")] int hours = 1,
        CancellationToken cancellationToken = default)
    {
        hours = Math.Clamp(hours, 1, 720);

        var kql = """
            dependencies
            | summarize
                TotalCalls    = count(),
                FailedCalls   = countif(success == false),
                AvgDurationMs = round(avg(duration), 1),
                MaxDurationMs = round(max(duration), 1)
              by type, target
            | extend FailureRate = round(todouble(FailedCalls) / TotalCalls * 100, 2)
            | order by FailedCalls desc
            """;

        return await ExecuteKql(kql, hours, cancellationToken);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private async Task<string> ExecuteKql(string kql, int hours, CancellationToken cancellationToken)
    {
        var response = await _logsClient.QueryWorkspaceAsync(
            _workspaceId, kql,
            new QueryTimeRange(TimeSpan.FromHours(hours)),
            cancellationToken: cancellationToken);

        return FormatTables(response.Value);
    }

    private static string FormatTables(LogsQueryResult result)
    {
        if (result.AllTables.Count == 0)
            return "No results returned.";

        var sb = new StringBuilder();

        foreach (var table in result.AllTables)
        {
            // Header row
            sb.AppendLine(string.Join(" | ", table.Columns.Select(c => c.Name)));
            sb.AppendLine(new string('-', 80));

            // Data rows
            foreach (var row in table.Rows)
                sb.AppendLine(string.Join(" | ", row.Select(cell => cell?.ToString() ?? "null")));

            if (table.Rows.Count == 0)
                sb.AppendLine("(no rows)");
        }

        return sb.ToString();
    }
}
