using StarAgent.McpServer.Shared.Models;

namespace StarAgent.McpServer.Shared.Services;

/// <summary>
///     Provides deterministic mock chart data for the live demo.
/// </summary>
public static class ChartDataService
{
    private static readonly Dictionary<string, ChartResult> _Entries = new Dictionary<string, ChartResult>(StringComparer.OrdinalIgnoreCase)
    {
        [CreateKey("Bohemian Rhapsody", "Queen", "Billboard Hot 100")] = new ChartResult(
            1,
            1,
            52,
            "Billboard Hot 100",
            "Bohemian Rhapsody",
            "Queen",
            false,
            "StarAgent demo seed"),
        [CreateKey("Smells Like Teen Spirit", "Nirvana", "Billboard Hot 100")] = new ChartResult(
            6,
            6,
            38,
            "Billboard Hot 100",
            "Smells Like Teen Spirit",
            "Nirvana",
            false,
            "StarAgent demo seed"),
        [CreateKey("Enter Sandman", "Metallica", "Billboard Hot 100")] = new ChartResult(
            16,
            16,
            20,
            "Billboard Hot 100",
            "Enter Sandman",
            "Metallica",
            false,
            "StarAgent demo seed")
    };

    public static ChartResult Lookup(string songTitle, string artist, string chart)
    {
        string safeSongTitle = NormalizeOrDefault(songTitle, "Unknown Song");
        string safeArtist = NormalizeOrDefault(artist, "Unknown Artist");
        string safeChart = NormalizeOrDefault(chart, "Billboard Hot 100");

        if (_Entries.TryGetValue(CreateKey(safeSongTitle, safeArtist, safeChart), out ChartResult? hit))
        {
            return hit;
        }

        int hash = HashCode.Combine(safeSongTitle, safeArtist, safeChart);
        int positiveHash = hash == int.MinValue
            ? int.MaxValue
            : Math.Abs(hash);

        int rank = (positiveHash % 100) + 1;
        int peak = Math.Max(1, rank - (positiveHash % 6));
        int weeks = 4 + (positiveHash % 60);

        return new ChartResult(
            rank,
            peak,
            weeks,
            safeChart,
            safeSongTitle,
            safeArtist,
            true,
            "StarAgent deterministic fallback");
    }

    private static string CreateKey(string songTitle, string artist, string chart)
    {
        return $"{songTitle.Trim().ToLowerInvariant()}|{artist.Trim().ToLowerInvariant()}|{chart.Trim().ToLowerInvariant()}";
    }

    private static string NormalizeOrDefault(string value, string fallback)
    {
        return string.IsNullOrWhiteSpace(value)
            ? fallback
            : value.Trim();
    }
}
