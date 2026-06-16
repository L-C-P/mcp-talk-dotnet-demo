# Slide 10 – HTTP Transport: Reference

## Slide text (EN)

### From stdio to Streamable HTTP – minimal changes

**NuGet:** `ModelContextProtocol.AspNetCore`
```shell
dotnet add package ModelContextProtocol.AspNetCore --prerelease
```

**Server (`Program.cs`):**
```csharp
using ModelContextProtocol.Server;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMcpServer()
    .WithHttpTransport(o => o.Stateless = true)  // <-- transport swap
    .WithToolsFromAssembly()
    .WithResourcesFromAssembly()
    .WithPromptsFromAssembly();
builder.AddDevUI(); // call before Build() — opens Inspector UI in the browser

var app = builder.Build();
app.MapMcp();           // registers /mcp endpoint
app.Run("http://localhost:3001");
```

**Connect a host via `mcp.json`:**
```json
{
  "servers": {
    "StarAgent": {
      "type": "http",
      "url": "http://localhost:3001/mcp"
    }
  }
}
```

### Transport comparison

| | stdio | HTTP `Stateless = true` | HTTP `Stateless = false` |
|---|---|---|---|
| Scope | Local process | Network-reachable | Network-reachable |
| Session | Persistent process | No session | Session-ID per client |
| In-memory state | Yes | No | Yes (per session) |
| Resource subscriptions | Yes | No | Yes |
| Sampling | Yes | No | Yes |
| Horizontal scaling | No (1 client) | Yes (stateless) | Needs sticky sessions |
| Auth | OS process boundary | Explicit (API key, AAD) | Explicit (API key, AAD) |
| Best for | Dev workstation | Cloud / serverless | Remote, long-lived clients |

---

## Speaker notes (DE)

- Folie ist reine Referenz – kein Live-Coding hier.
- Hauptaussage: Die Tool-/Resource-/Prompt-Implementierungen bleiben 1:1 identisch. Nur Program.cs ändert sich.
- `Stateless = true` passt perfekt zu serverless-Deployments wie Azure Functions (kommt gleich).
- `app.MapMcp()` registriert den `/mcp`-Endpunkt – analog zu `app.MapControllers()` oder `app.MapHub()`.
- Auth-Hinweis: Bei HTTP-Transport ist Authentifizierung Pflicht in Produktion. Typisch: Azure AD, API-Keys oder OAuth.
- Sampling erklären: Sampling ist der umgekehrte Weg – normalerweise ruft der Host/Client den Server auf. Beim Sampling dreht der Server den Spieß um und bittet seinerseits den Host, das LLM mit einer bestimmten Anfrage aufzurufen und das Ergebnis zurückzugeben. Beispiel: Ein Tool läuft, merkt dass es zusätzlichen Kontext vom Modell braucht, und löst über den Host eine neue LLM-Anfrage aus – ohne dass der Nutzer direkt eingreifen muss. Das funktioniert nur mit einer persistenten Verbindung (stdio oder HTTP stateful).
