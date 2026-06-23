using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using StarAgent.McpServer.Shared.Models;
using StarAgent.McpServer.Shared.Services;
using System.ComponentModel;
using System.Text.Json;

namespace StarAgent.McpServer.Shared.Tools;

/// <summary>
///     MCP tool endpoints for chart lookup and venue booking.
/// </summary>
[McpServerToolType]
public static class ChartTools
{
    [McpServerTool(Name = "get_chart_position")]
    [Description("Returns the chart position of a song on a given chart.")]
    public static ChartResult GetChartPosition([Description("Song title")] string songTitle,
                                               [Description("Artist name")] string artist,
                                               [Description("Chart name")] string chart = "Billboard Hot 100")
    {
        return ChartDataService.Lookup(songTitle, artist, chart);
    }

    /// <summary>
    ///     Books a concert venue for an artist. If city, date, or capacity are not provided,
    ///     the server uses MCP Elicitation to ask the user for the missing details.
    /// </summary>
    [McpServerTool(Name = "book_venue")]
    [Description("Books a concert venue for an artist. If city, date, or capacity are missing, the server will ask the user.")]
    public static async Task<BookingResult> BookVenue(
        ModelContextProtocol.Server.McpServer server,
        [Description("Artist name")] string artist,
        [Description("City")] string? city = null,
        [Description("Date (yyyy-MM-dd)")] string? date = null,
        [Description("Required capacity")] int? capacity = null,
        CancellationToken cancellationToken = default)
    {
        bool needsElicitation = string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(date) || capacity == null || capacity <= 0;

        if (needsElicitation)
        {
            if (server.ClientCapabilities?.Elicitation == null)
            {
                return new BookingResult(
                    "rejected",
                    artist,
                    city ?? "Unknown",
                    date ?? "Unknown",
                    capacity ?? 0,
                    null,
                    null,
                    "Missing required parameters (city, date, capacity) and client does not support elicitation.");
            }

            var schema = new ElicitRequestParams.RequestSchema();

            if (string.IsNullOrWhiteSpace(city))
            {
                schema.Properties["city"] = new ElicitRequestParams.UntitledSingleSelectEnumSchema
                {
                    Description = "Select a city for the concert",
                    Enum = ["London", "Berlin", "Munich", "Hamburg"]
                };
            }

            if (string.IsNullOrWhiteSpace(date))
            {
                schema.Properties["date"] = new ElicitRequestParams.StringSchema
                {
                    Description = "Concert date in yyyy-MM-dd format"
                };
            }

            if (capacity == null || capacity <= 0)
            {
                schema.Properties["capacity"] = new ElicitRequestParams.NumberSchema
                {
                    Description = "Required venue capacity (minimum number of seats)"
                };
            }

            var result = await server.ElicitAsync(new ElicitRequestParams
            {
                Message = "Please provide the missing booking details.",
                RequestedSchema = schema
            }, cancellationToken);

            if (result.Action != "accept" || result.Content == null)
            {
                return new BookingResult(
                    "cancelled",
                    artist,
                    city ?? "Unknown",
                    date ?? "Unknown",
                    capacity ?? 0,
                    null,
                    null,
                    "Booking was cancelled by the user.");
            }

            if (string.IsNullOrWhiteSpace(city) && result.Content.TryGetValue("city", out JsonElement cityElement) && cityElement.ValueKind == JsonValueKind.String)
                city = cityElement.GetString();

            if (string.IsNullOrWhiteSpace(date) && result.Content.TryGetValue("date", out JsonElement dateElement) && dateElement.ValueKind == JsonValueKind.String)
                date = dateElement.GetString();

            if ((capacity == null || capacity <= 0) && result.Content.TryGetValue("capacity", out JsonElement capacityElement) && capacityElement.ValueKind == JsonValueKind.Number)
                capacity = (int)capacityElement.GetDouble();
        }

        return VenueService.Book(artist, city ?? "Unknown", date ?? "Unknown", capacity ?? 0);
    }
}
