# Slide 10 – HTTP Transport: Reference

## Slide text (EN)

### From stdio to Streamable HTTP – minimal changes

**Server (`Program.cs`):**
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMcpServer()
    .WithHttpTransport(o => o.Stateless = true)  // <-- transport swap
    .WithToolsFromAssembly()
    .WithResourcesFromAssembly()
    .WithPromptsFromAssembly();

var app = builder.Build();
app.MapMcp();           // registers /mcp endpoint
app.Run("http://localhost:3001");
```

**Host/client – discover and call:**
```csharp
var transport = new HttpClientTransport(new HttpClientTransportOptions
{
    Endpoint      = new Uri("http://localhost:3001/mcp"),
    TransportMode = HttpTransportMode.StreamableHttp
});

await using var client = await McpClient.CreateAsync(transport);
var tools  = await client.ListToolsAsync();
var result = await client.CallToolAsync("get_chart_position",
                 new { songTitle = "Bohemian Rhapsody", artist = "Queen" });
```

### Key differences vs stdio

| | stdio | Streamable HTTP |
|---|---|---|
| Scope | Local process | Network-reachable |
| State | Stateful session | Stateless (per request) |
| Auth | OS process boundary | Requires explicit auth (API key, AAD) |
| Best for | Dev workstation | Shared / cloud deployment |

---

## Speaker notes (DE)

- Folie ist reine Referenz – kein Live-Coding hier.
- Hauptaussage: Die Tool-/Resource-/Prompt-Implementierungen bleiben 1:1 identisch. Nur Program.cs ändert sich.
- `Stateless = true` passt perfekt zu serverless-Deployments wie Azure Functions (kommt gleich).
- `app.MapMcp()` registriert den `/mcp`-Endpunkt – analog zu `app.MapControllers()` oder `app.MapHub()`.
- Auth-Hinweis: Bei HTTP-Transport ist Authentifizierung Pflicht in Produktion. Typisch: Azure AD, API-Keys oder OAuth.
