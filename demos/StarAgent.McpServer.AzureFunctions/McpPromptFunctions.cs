using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Mcp;

namespace StarAgent.McpServer.AzureFunctions;

/// <summary>
///     MCP prompt triggers hosted by Azure Functions.
/// </summary>
public class McpPromptFunctions
{
    [Function(nameof(GetConcertPressReleasePrompt))]
    public string GetConcertPressReleasePrompt([McpPromptTrigger("concert_press_release", Description = "Generates a dramatic concert press release.")] PromptInvocationContext context)
    {
        string artist = GetArgument(context, "artist", "the featured artist");
        string venue = GetArgument(context, "venue", "the main stage");
        string date = GetArgument(context, "date", "TBA");
        string tourName = GetArgument(context, "tourName", "The Legend Returns Tour");

        return $"Write a dramatic press release for {artist} performing '{tourName}' at {venue} on {date}. The legend returns.";
    }

    private static string GetArgument(PromptInvocationContext context, string key, string fallback)
    {
        if (context.Arguments is not null &&
            context.Arguments.TryGetValue(key, out string? value) &&
            !string.IsNullOrWhiteSpace(value))
        {
            return value.Trim();
        }

        return fallback;
    }
}
