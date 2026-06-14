# Slide 08 – Microsoft MCP SDK for .NET

## Slide text (EN)

### NuGet package

```shell
dotnet add package ModelContextProtocol --prerelease
```

### Server-side: attribute-driven registration

```csharp
// Mark a class as a Tool container
[McpServerToolType]
public class ChartTools
{
    [McpServerTool(Name = "get_chart_position")]
    [Description("Returns the chart position of a song.")]
    public static ChartResult GetChartPosition(
        [Description("Song title")] string songTitle,
        [Description("Artist name")] string artist,
        [Description("Chart name")] string chart = "Billboard Hot 100")
    { ... }
}

// Resources and Prompts follow the same pattern
[McpServerResourceType]   →   [McpServerResource(UriTemplate = "rider://artist/{name}")]
[McpServerPromptType]     →   [McpServerPrompt(Name = "concert_press_release")]
```

### Host-side: discover and invoke

```csharp
await using var client = await McpClient.CreateAsync(transport);

var tools  = await client.ListToolsAsync();
var result = await client.CallToolAsync("get_chart_position",
    new { songTitle = "Bohemian Rhapsody", artist = "Queen" });
```

---

## Speaker notes (DE)

- SDK einordnen: Preview-Paket, aber aktiv von Microsoft entwickelt. API-Namen können sich noch leicht ändern – immer Paketversion prüfen.
- Attribute-Ansatz betonen: Wer .NET kennt, fühlt sich sofort zu Hause. Kein Boilerplate, kein manuelles JSON-Parsing.
- Description-Attribute sind entscheidend: Sie landen direkt im Tool-Schema, das das LLM sieht. Je klarer die Description, desto besser die Tool-Auswahl durch das Modell.
- Host-Seite zeigen: ListToolsAsync gibt die Discovery zurück, CallToolAsync führt aus. Genau das, was wir in Folie 6 als JSON-RPC gesehen haben – jetzt als typisierter .NET-Aufruf.
- Überleitung zur Demo: Genug Theorie. Bauen wir das jetzt live.
