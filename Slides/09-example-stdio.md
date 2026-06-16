# Slide 09 – Live Demo: StarAgent MCP Server (stdio)

## Slide text (EN)

### 🎸 StarAgent – local stdio server (live)

**Bootstrap (`Program.cs`):**
```csharp
var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly()
    .WithResourcesFromAssembly()
    .WithPromptsFromAssembly();

await builder.Build().RunAsync();
```

**Tools – `get_chart_position` + `book_venue`:**
```csharp
[McpServerToolType]
public class ChartTools
{
    [McpServerTool(Name = "get_chart_position")]
    [Description("Returns the chart position of a song on a given chart.")]
    public static ChartResult GetChartPosition(
        [Description("Song title")] string songTitle,
        [Description("Artist name")] string artist,
        [Description("Chart name")] string chart = "Billboard Hot 100")
        => ChartDataService.Lookup(songTitle, artist, chart);

    [McpServerTool(Name = "book_venue")]
    [Description("Books a concert venue for an artist on a given date.")]
    public static BookingResult BookVenue(
        [Description("Artist name")] string artist,
        [Description("City")] string city,
        [Description("Date (yyyy-MM-dd)")] string date,
        [Description("Required capacity")] int capacity)
        => VenueService.Book(artist, city, date, capacity);
}
```

**Resource – `rider://artist/{name}`:**
```csharp
[McpServerResourceType]
public class RiderResources
{
    [McpServerResource(UriTemplate = "rider://artist/{name}",
                       Name = "artist_rider", MimeType = "application/json")]
    [Description("Returns the backstage rider for an artist. " +
                 "Includes stage requirements, catering, and special requests.")]
    public static string GetRider(
        [Description("Artist slug, e.g. van-halen or foo-fighters")] string name)
        => RiderDataService.Load(name);
}
```

**Prompt – `concert_press_release`:**
```csharp
[McpServerPromptType]
public class PressReleasePrompts
{
    [McpServerPrompt(Name = "concert_press_release")]
    [Description("Generates a dramatic concert press release.")]
    public static IEnumerable<PromptMessage> ConcertPressRelease(
        [Description("Artist name")]  string artist,
        [Description("Venue name")]   string venue,
        [Description("Concert date")] string date,
        [Description("Tour name")]    string tourName)
        => [new PromptMessage(Role.User,
            $"Write a dramatic press release for {artist} performing '{tourName}' " +
            $"at {venue} on {date}. The legend returns.")];
}
```

---

## Speaker notes (DE)

- Reihenfolge live: zuerst Program.cs zeigen und erklären, dann die drei Klassen nacheinander implementieren.
- Logging-Hinweis: Bei stdio läuft die Protokollkommunikation über stdout. Logging immer auf stderr oder in eine Datei umleiten, damit keine Lognachrichten das Protokoll stören.
- `WithToolsFromAssembly()` / `WithResourcesFromAssembly()` / `WithPromptsFromAssembly()` – alle Klassen mit den entsprechenden Attributen im Assembly werden automatisch registriert.
- Nach dem Bauen: Server im MCP-Host (z. B. VS Code / Claude Desktop) einbinden und live eine Chart-Abfrage und einen Rider-Abruf zeigen.
- Rider-Punchline: Van Halen öffnen → „Absolutely NO brown M&Ms" live im Chat auftauchen lassen. 🤘
- Prompt zeigen: concert_press_release aufrufen, dramatische Pressemitteilung für „Queen Tribute Band at Wembley" generieren lassen.
- Debug-Tipp für stdio: MCP Inspector via `npx @modelcontextprotocol/inspector` – öffnet eine Browser-UI zum manuellen Testen der Tools, Resources und Prompts ohne Host.
