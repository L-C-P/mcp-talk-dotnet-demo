namespace StarAgent.McpServer.Shared.Models;

/// <summary>
///     Represents a chart lookup result for a specific song on a specific chart.
/// </summary>
public sealed record ChartResult(
    int Rank,
    int Peak,
    int Weeks,
    string Chart,
    string SongTitle,
    string Artist,
    bool IsEstimated,
    string DataSource);
