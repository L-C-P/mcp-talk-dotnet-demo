using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;

namespace StarAgent.McpServer.AzureFunctions;

/// <summary>
///     HTTP endpoints exposing well-known MCP discovery metadata.
/// </summary>
public class WellKnownDiscoveryFunctions
{
    [Function(nameof(GetMcpJson))]
    public async Task<HttpResponseData> GetMcpJson([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ".well-known/mcp.json")] HttpRequestData request)
    {
        var payload = new
        {
            mcpServers = new Dictionary<string, object>
            {
                ["star-agent"] = new
                {
                    url = "http://localhost:7071/runtime/webhooks/mcp",
                    name = "StarAgent",
                    description = "AI tour manager for concerts and artists"
                }
            }
        };

        HttpResponseData response = request.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");
        await response.WriteStringAsync(JsonSerializer.Serialize(payload));

        return response;
    }

    [Function(nameof(GetRegistryServerJson))]
    public async Task<HttpResponseData> GetRegistryServerJson([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ".well-known/mcp/server.json")] HttpRequestData request)
    {
        var payload = new
        {
            name = "StarAgent",
            description = "AI tour manager for concerts and artists",
            url = "http://localhost:7071/runtime/webhooks/mcp",
            categories = new[]
            {
                "entertainment", "events"
            }
        };

        HttpResponseData response = request.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");
        await response.WriteStringAsync(JsonSerializer.Serialize(payload));

        return response;
    }
}
