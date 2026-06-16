---
theme: default
title: "Behind the Scenes: MCP"
author: "Denis Sowa"
colorSchema: light
themeConfig:
    primary: '#216ec7'
fonts:
    sans: Fira Sans
    serif: Cambria
info: |
    Business Line Meeting · "The Show must go on!" · 45 min · EN slides / DE spoken
drawings:
    persist: false
transition: slide-left
duration: 45min
timer: countdown
wakeLock: false
routerMode: hash
layout: cover
background: /assets/BLMeetingBackground.png
---

# Business<br/>Line Meeting

*The show must go on.*<br/>
*Oberhausen. 2026.*

---
layout: intro
hideInToc: true
---

# Behind the Scenes: MCP

The Director Between AI and Enterprise Data

<!--
-->

---
layout: section
background: /assets/SectionBackground.png
hideInToc: true
---

# Denis Sowa

Architect, AI-Ambassador<br/>
BL Microsoft, Hannover

<!--
-->

---
layout: center
class: toc
hideInToc: true
---

# _Today's Setlist_

<Toc :columns="2" />

<!--
- Zeit: 2 min
- Kurz die Energie des Event-Themas aufgreifen: Heute sind wir alle Stars – und StarAgent managed die Tour.
- Erwartung setzen: Kein reiner Theorie-Vortrag. Am Ende läuft ein echter MCP-Server live.
- Ziel benennen: Jede Person hier soll danach in der Lage sein, den Kommunikationsfluss zwischen LLM und MCP zu erklären – und wissen, wie sie selbst einen Server bauen kann.
-->

---
layout: center
hideInToc: true
---

# DEMO

---
layout: terminal
cast: /assets/casts/demo.cast
---

---
transition: slide-up
---

# Why We Needed MCP

### The world before MCP

| Problem                                   | Reality                        |
|-------------------------------------------|--------------------------------|
| Every AI integration was custom-built     | Glue code everywhere           |
| Connectors not portable across hosts      | Rewrite for each AI app        |
| No standard for security or lifecycle     | Every team reinvents the wheel |
| Maintenance cost grew with each new model | Fragile, tightly coupled       |

> MCP standardizes the contract between AI and the outside world.

<!--
- Zeit: 2 min
- Pain Points aus dem eigenen Umfeld nennen: Wer hat schon mal einen eigenen Connector für ein LLM gebaut?
- Analogie: Vor USB gab es für jedes Gerät einen anderen Stecker. MCP ist der USB-Standard für AI-Integrationen.
- Kernbotschaft: MCP ist kein Framework, kein Produkt – es ist ein offenes Protokoll, das den Vertrag zwischen AI-Host und externer Fähigkeit definiert.
-->

---
zoom: 0.95
---

# Useful MCP Servers: Practical Examples

| Server                | What it exposes                                |
|-----------------------|------------------------------------------------|
| **Context7**          | Up-to-date library docs and code examples      |
| **Microsoft Learn**   | Official Microsoft / Azure documentation       |
| **GitHub**            | Repos, issues, pull requests, code search      |
| **Azure**             | Azure resources, subscriptions, deployments    |
| **Jira / Confluence** | Tickets, pages, project data                   |
| **Azure DevOps**      | Work items, pipelines, repos, boards           |
| **Playwright**        | Browser automation and web scraping            |
| **Chrome DevTools**   | Live browser inspection, console, network, DOM |

<!--
- Zeit: 2 min
- Hinweis: Die Verzeichnis-URLs sind später auf der Folie "Where to go next" aufgeführt.
- Überleitung: Was genau ist dieses Protokoll?
-->

---

# What Is MCP?

### Model Context Protocol (MCP)

> An open standard that defines how AI applications securely and structurally connect to tools and data sources.

- **Wire protocol:** JSON-RPC 2.0 – both sides speak structured text
- **Capability model:** Tools · Resources · Prompts
- **Lifecycle management:** connection setup, capability discovery, invocation, teardown
- **Interoperability:** one server works with any MCP-compatible host
- **Transports:** `stdio` for local processes · `Streamable HTTP` for remote services

<!--
- Zeit: 2 min
- MCP als Protokoll einordnen, nicht als Bibliothek oder Framework.
- Herkunft: MCP wurde von **Anthropic** entwickelt und im **November 2024** als offener Standard veröffentlicht. Seitdem wird es von Microsoft, GitHub, Google und zahlreichen anderen Unternehmen aktiv unterstützt und weiterentwickelt.
- JSON-RPC 2.0 hervorheben: Host und Server tauschen schlicht strukturierten Text aus – dazu gleich mehr.
- Transport kurz erwähnen: lokal läuft es über stdio (Standard-Ein-/Ausgabe), remote über HTTP. Details kommen im Architektur-Diagramm.
- Interoperabilität betonen: ein MCP-Server in .NET funktioniert mit GitHub Copilot, Claude Desktop, VS Code und jedem anderen MCP-Host.
-->

---
transition: slide-up
---

# MCP Architecture

```mermaid {scale: 0.65}
flowchart LR
    subgraph YC["Your Machine"]
        H["Host<br/>(IDE / Agent Shell)"]
        C["MCP Client"]
        S1["MCP Server A<br/>(local · stdio)"]
        S2["MCP Server B<br/>(local · stdio)"]
        DS1[("Local<br/>Data Source")]
        H <--> C
        C <-->|" JSON-RPC 2.0<br/>(stdio) "| S1
        C <-->|" JSON-RPC 2.0<br/>(stdio) "| S2
        S1 <--> DS1
    end
    subgraph Remote["Remote"]
        RS["MCP Server C<br/>(remote · HTTP)"]
    end
    subgraph Internet["External Systems"]
        DS2[("Database /<br/>File Store")]
        RSVC[("Remote<br/>Service / API")]
    end
    S2 <-->|" Web APIs "| DS2
    C <-->|" JSON-RPC 2.0<br/>(Streamable HTTP) "| RS
    RS <-->|" Web APIs "| RSVC
```

<!--
- Zeit: 2 min
- Die drei Rollen klar abgrenzen: Host = AI-App, Client = Protokollschicht im Host, Server = Fähigkeiten-Anbieter.
- Wichtiger Punkt: Host und Server können unabhängig voneinander entwickelt werden – das ist die Stärke des Standards.
- Der Host kann mehrere Clients gleichzeitig nutzen.
- Diagramm erläutern: links lokal über stdio (einfach, schnell, für Entwicklung), rechts remote über HTTP (produktionstauglich, skalierbar).
- Beispiel aus der Praxis: VS Code mit GitHub Copilot ist der Host + Client. Unser StarAgent-Server ist der MCP Server.
-->

---
hideInToc: true
---

# MCP Architecture

### Three roles – clear responsibilities

- **Host:** the AI application (IDE, agent shell, chat client)
    - Manages the LLM conversation
    - Decides which capabilities the model may use
- **MCP Client:** protocol connector embedded in the host
    - Speaks MCP to one or more servers
    - Translates server capabilities into LLM-usable function definitions
- **MCP Server:** exposes capabilities and wraps external systems
    - Implements Tools, Resources, and/or Prompts
    - Completely independent of host and model

<!--
- Zeit: 2 min
- Die drei Rollen klar abgrenzen: Host = AI-App, Client = Protokollschicht im Host, Server = Fähigkeiten-Anbieter.
- Wichtiger Punkt: Host und Server können unabhängig voneinander entwickelt werden – das ist die Stärke des Standards.
- Diagramm erläutern: links lokal über stdio (einfach, schnell, für Entwicklung), rechts remote über HTTP (produktionstauglich, skalierbar).
- Beispiel aus der Praxis: VS Code mit GitHub Copilot ist der Host + Client. Unser StarAgent-Server ist der MCP Server.
-->

---
transition: slide-up
---

# Capabilities: Tools, Resources, Prompts

### Three primitives – three distinct concerns

| Primitive    | Purpose                                            | StarAgent example                                               |
|--------------|----------------------------------------------------|-----------------------------------------------------------------|
| **Tool**     | Executable function · model calls it, host runs it | `get_chart_position` – where is a song on the charts?           |
| **Resource** | Read-only data, addressable by URI                 | `rider://artist/van-halen` – Van Halen's backstage requirements |
| **Prompt**   | Reusable prompt template with placeholders         | `concert_press_release` – generate a dramatic tour announcement |

<!--
- Zeit: 2 min
- Die Semantik der drei Primitive präzise machen: Tool = execute, Resource = read, Prompt = orchestrate.
- StarAgent-Beispiele direkt zeigen: Wir bauen gleich alle drei live.
- Rider erklären: Ein Rider ist das echte Dokument, das jeder Künstler vor einem Konzert einreicht – Bühnenanforderungen, Catering, Sonderwünsche. Van Halens berühmteste Forderung: „Absolutely NO brown M&Ms.“ Das ist ein perfektes Beispiel für eine Resource – stabil, adressierbar, read-only.
- Enterprise-Brücke: Statt Rider → euer Konfigurations-Dokument, euer OpenAPI-Spec, euer Feature-Spec. Das Prinzip ist identisch.
-->

---
hideInToc: true
---

# Capabilities: Tools, Resources, Prompts

### Decision guide

- **Tool** → the model needs to *do* something or fetch dynamic data
- **Resource** → stable, readable document or data set (like a file or config)
- **Prompt** → standardized, repeatable workflow the model should follow

<!--
- Zeit: 1 min
- Governance-Hinweis: Die drei Primitiv-Typen haben unterschiedliche Risikoprofile – Tools können Seiteneffekte haben, Resources und Prompts sind read-only.
-->

---
transition: slide-up
---

# How LLM and MCP Actually Talk

### The LLM never sees MCP – the host is the translator

The **host** is embedded in the client application (Claude Code, Warp, GitHub Copilot, …).
It knows two languages: **MCP** on one side, and the **LLM's native format** on the other.

```text
MCP Server (.NET)
    ↕  always: MCP / JSON-RPC 2.0
Host (embedded in client)
    ↕  translated to the LLM's format:
        Claude Code / Warp  →  Anthropic Tool Use  →  injected into system prompt
        GitHub Copilot       →  OpenAI Function Calling
        Gemini               →  Google Function Calling
LLM
```

The MCP server never knows which LLM or client is on the other end.
One server works with every MCP-compatible host – the host handles the translation.
<!--
- Zeit: 2 min
- Kernbotschaft deutlich machen: LLM und MCP-Server sprechen **nie direkt** miteinander. Der Host ist immer der Vermittler und die Kontrollinstanz.
- JSON-RPC 2.0 ist kein Hexenwerk – es sind strukturierte Textnachrichten mit `method`, `params` und `result`.
- Schritt 1–2 folgt auf den nächsten Folien: Der Host fragt den Server nach Fähigkeiten und übersetzt sie in Function-Definitions für das Modell.
-->

---
hideInToc: true
zoom: 0.75
transition: slide-up
---

# It's just text – structured text

### Step 1 – Host discovers tools from MCP server (JSON-RPC)

```json
{
    "jsonrpc": "2.0",
    "id": 1,
    "result": {
        "tools": [
            {
                "name": "get_chart_position",
                "description": "Returns the chart position of a song.",
                "inputSchema": {
                    "type": "object",
                    "properties": {
                        "song_title": {
                            "type": "string"
                        },
                        "artist": {
                            "type": "string"
                        },
                        "chart": {
                            "type": "string",
                            "default": "Billboard Hot 100"
                        }
                    },
                    "required": [
                        "song_title",
                        "artist"
                    ]
                }
            }
        ]
    }
}
```

<!--
- Zeit: 3 min
- Schritt 1–2 ist der Discovery-Vorgang: Der Host fragt den Server nach seinen Fähigkeiten und übersetzt das in Function-Definitions für das Modell.
- Highlight: Das LLM „sieht“ nur die Tool-Schemata – es weiß nicht, ob dahinter .NET, Python oder ein Toaster steckt.
-->

---
hideInToc: true
zoom: 0.85
transition: slide-up
---

# It's just text – structured text

### Step 2 – Host passes tool schema to LLM as a callable function

```json
{
    "type": "function",
    "function": {
        "name": "get_chart_position",
        "description": "Returns the chart position of a song.",
        "parameters": {
            "type": "object",
            "properties": {
                "song_title": {
                    "type": "string"
                },
                "artist": {
                    "type": "string"
                },
                "chart": {
                    "type": "string"
                }
            },
            "required": [
                "song_title",
                "artist"
            ]
        }
    }
}
```

<!--
- Zeit: 3 min
- Schritt 1–2 ist der Discovery-Vorgang: Der Host fragt den Server nach seinen Fähigkeiten und übersetzt das in Function-Definitions für das Modell.
- Highlight: Das LLM „sieht“ nur die Tool-Schemata – es weiß nicht, ob dahinter .NET, Python oder ein Toaster steckt.
-->

---
hideInToc: true
transition: slide-up
---

# It's just text – structured text

### Step 3 – LLM responds with a tool call

```json
{
    "name": "get_chart_position",
    "arguments": {
        "song_title": "Bohemian Rhapsody",
        "artist": "Queen"
    }
}
```

<!--
- Zeit: 2 min
- Schritt 3: Das LLM entscheidet, ob es ein Tool aufrufen will – es gibt einfach JSON zurück. Keine Magie.
- Schritt 4–5: Host führt den Tool-Call aus (policy check, ggf. user approval) und schickt das Ergebnis als neuen Context an das Modell.
-->

---
hideInToc: true
transition: slide-up
---

# It's just text – structured text

### Step 4 – Host sends `tools/call` to MCP server

```json
{
    "jsonrpc": "2.0",
    "id": 2,
    "method": "tools/call",
    "params": {
        "name": "get_chart_position",
        "arguments": {
            "song_title": "Bohemian Rhapsody",
            "artist": "Queen"
        }
    }
}
```

<!--
- Zeit: 2 min
- Schritt 3: Das LLM entscheidet, ob es ein Tool aufrufen will – es gibt einfach JSON zurück. Keine Magie.
- Schritt 4–5: Host führt den Tool-Call aus (policy check, ggf. user approval) und schickt das Ergebnis als neuen Context an das Modell.
-->

---
hideInToc: true
---

# It's just text – structured text

### Step 5 – MCP server returns result → host feeds it back to LLM

```json
{
    "jsonrpc": "2.0",
    "id": 2,
    "result": {
        "content": [
            {
                "type": "text",
                "text": "{\"rank\":1,\"peak\":1,\"weeks\":52,\"chart\":\"Billboard Hot 100\"}"
            }
        ]
    }
}
```

<!--
- Zeit: 2 min
- Schritt 3: Das LLM entscheidet, ob es ein Tool aufrufen will – es gibt einfach JSON zurück. Keine Magie.
- Schritt 4–5: Host führt den Tool-Call aus (policy check, ggf. user approval) und schickt das Ergebnis als neuen Context an das Modell.
-->

---
transition: slide-up
hideInToc: true
---

# Discovery & Runtime Sequence

### End-to-end runtime flow

```mermaid {scale: 0.6}
sequenceDiagram
    actor User
    participant Host
    participant LLM
    participant MCP as MCP Server
    User ->> Host: "Where does Bohemian Rhapsody<br/>rank on the charts?"
    Host ->> LLM: User message + available tool definitions
    LLM -->> Host: Tool call request: get_chart_position(...)
    Host ->> Host: Policy check / optional user approval
    Host ->> MCP: tools/call · get_chart_position
    MCP -->> Host: { rank: 1, peak: 1, weeks: 52 }
    Host ->> LLM: Tool result as new context
    LLM -->> Host: Final answer
    Host -->> User: "Bohemian Rhapsody is#1 – as always."
```

<!--
- Zeit: 2 min
- Lifecycle-Tabelle nur kurz streifen: Diese Calls gibt es, der Host verwaltet sie automatisch. Man muss sie nicht selbst implementieren – das SDK erledigt das.
- Sequenzdiagramm ist das Herzstück: Hier wird sichtbar, dass der Host die Kontrolle behält. Das LLM macht einen Vorschlag (Tool Call), aber der Host entscheidet, ob er ausgeführt wird.
- Wichtige Botschaft: Der Host ist die Sicherheitsinstanz – nicht das LLM.
- Punchline zum Diagramm: Das letzte Ergebnis – „Bohemian Rhapsody ist #1 – wie immer“ – ist unser Mock-Verhalten für die Demo. Kommt gleich live.
-->

---
hideInToc: true
---

# Discovery & Runtime Sequence

### MCP lifecycle calls (for reference)

| Phase      | Call                                             | Direction       |
|------------|--------------------------------------------------|-----------------|
| Setup      | `initialize` + `notifications/initialized`       | Client → Server |
| Discovery  | `tools/list` · `resources/list` · `prompts/list` | Client → Server |
| Invocation | `tools/call` · `resources/read` · `prompts/get`  | Client → Server |

<!--
- Zeit: 2 min
- Lifecycle-Tabelle nur kurz streifen: Diese Calls gibt es, der Host verwaltet sie automatisch. Man muss sie nicht selbst implementieren – das SDK erledigt das.
- Sequenzdiagramm ist das Herzstück: Hier wird sichtbar, dass der Host die Kontrolle behält. Das LLM macht einen Vorschlag (Tool Call), aber der Host entscheidet, ob er ausgeführt wird.
- Wichtige Botschaft: Der Host ist die Sicherheitsinstanz – nicht das LLM.
- Punchline zum Diagramm: Das letzte Ergebnis – „Bohemian Rhapsody ist #1 – wie immer“ – ist unser Mock-Verhalten für die Demo. Kommt gleich live.
-->

---

# Microsoft MCP SDK for .NET

### Getting started – three options

**Option A: Project template (recommended for new projects)**

```shell
dotnet new install Microsoft.McpServer.ProjectTemplates
dotnet new mcpserver -n StarAgent.McpServer
```

**Option B: Visual Studio**

- New Project → search for "MCP" → select the MCP Server template

**Option C: Add to an existing project**

```shell
dotnet add package ModelContextProtocol
```

### Register the server in your MCP host (`mcp.json`)

```json
{
    "servers": {
        "StarAgent": {
            "type": "stdio",
            "command": "dotnet",
            "args": [
                "run",
                "--project",
                "<path-to-project>"
            ]
        }
    }
}
```

<!--
- Zeit: 2 min
- SDK einordnen: Preview-Paket, aber aktiv von Microsoft entwickelt. API-Namen können sich noch leicht ändern – immer Paketversion prüfen.
- Überleitung zur nächsten Folie: Die eigentliche Registrierung der Capabilities läuft attributbasiert.
-->

---
hideInToc: true
---

# Microsoft MCP SDK for .NET

### Server-side: attribute-driven registration

```csharp
using ModelContextProtocol.Server;
using System.ComponentModel;

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

<!--
- Zeit: 2 min
- Attribute-Ansatz betonen: Wer .NET kennt, fühlt sich sofort zu Hause. Kein Boilerplate, kein manuelles JSON-Parsing.
- Description-Attribute sind entscheidend: Sie landen direkt im Tool-Schema, das das LLM sieht. Je klarer die Description, desto besser die Tool-Auswahl durch das Modell.
- Host-Seite zeigen: ListToolsAsync gibt die Discovery zurück, CallToolAsync führt aus. Genau das, was wir in Folie 6 als JSON-RPC gesehen haben – jetzt als typisierter .NET-Aufruf.
- Überleitung zur Demo: Genug Theorie. Bauen wir das jetzt live.
-->

---

# Live Demo: StarAgent MCP Server (stdio)

### Bootstrap (`Program.cs`)

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

### Demo capabilities

- Tool: `get_chart_position`
- Resource: `rider://artist/{name}`
- Prompt: `concert_press_release`

<!--
- Zeit: 8 min
- Reihenfolge live: zuerst Program.cs zeigen und erklären, dann die drei Klassen nacheinander implementieren.
- Logging-Hinweis: Bei stdio läuft die Protokollkommunikation über stdout. Logging immer auf stderr oder in eine Datei umleiten, damit keine Lognachrichten das Protokoll stören.
- `WithToolsFromAssembly()` / `WithResourcesFromAssembly()` / `WithPromptsFromAssembly()` – alle Klassen mit den entsprechenden Attributen im Assembly werden automatisch registriert.
- Nach dem Bauen: Server im MCP-Host (z. B. VS Code / Claude Desktop) einbinden und live eine Chart-Abfrage und einen Rider-Abruf zeigen.
- Rider-Punchline: Van Halen öffnen → „Absolutely NO brown M&Ms“ live im Chat auftauchen lassen. 🤘
- Prompt zeigen: `concert_press_release` aufrufen, dramatische Pressemitteilung für „Queen Tribute Band at Wembley“ generieren lassen.
- Debug-Tipp für stdio: MCP Inspector via `npx @modelcontextprotocol/inspector` – öffnet eine Browser-UI zum manuellen Testen der Tools, Resources und Prompts ohne Host.
-->

---

# HTTP Transport: Reference

### From stdio to Streamable HTTP – minimal changes

**NuGet:** `ModelContextProtocol.AspNetCore`

```shell
dotnet add package ModelContextProtocol.AspNetCore
```

**Server (`Program.cs`)**

```csharp {monaco-diff}  {height:'250px'}
var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly()
    .WithResourcesFromAssembly()
    .WithPromptsFromAssembly();

var app = builder.Build();
await app.RunAsync();
~~~
using ModelContextProtocol.Server;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly()
    .WithResourcesFromAssembly()
    .WithPromptsFromAssembly();
builder.AddDevUI();

var app = builder.Build();
app.MapMcp();
await app.RunAsync("http://localhost:3001");
```

**Connect a host via `mcp.json`**

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

<!--
- Zeit: 2 min
- Folie ist reine Referenz – kein Live-Coding hier.
- Hauptaussage: Die Tool-/Resource-/Prompt-Implementierungen bleiben 1:1 identisch. Nur `Program.cs` ändert sich.
- `Stateless = true` passt perfekt zu serverless-Deployments wie Azure Functions (kommt gleich).
- `app.MapMcp()` registriert den `/mcp`-Endpunkt – analog zu `app.MapControllers()` oder `app.MapHub()`.
- Auth-Hinweis: Bei HTTP-Transport ist Authentifizierung Pflicht in Produktion. Typisch: Azure AD, API-Keys oder OAuth.
- Sampling erklären: Sampling ist der umgekehrte Weg – normalerweise ruft der Host/Client den Server auf. Beim Sampling dreht der Server den Spieß um und bittet seinerseits den Host, das LLM mit einer bestimmten Anfrage aufzurufen und das Ergebnis zurückzugeben. Beispiel: Ein Tool läuft, merkt dass es zusätzlichen Kontext vom Modell braucht, und löst über den Host eine neue LLM-Anfrage aus – ohne dass der Nutzer direkt eingreifen muss. Das funktioniert nur mit einer persistenten Verbindung (stdio oder HTTP stateful).
-->

---

# MCP on Azure Functions

### Hosting an MCP server serverless – Azure Functions (isolated worker)

**NuGet packages**

```shell
dotnet add package ModelContextProtocol --prerelease
dotnet add package Microsoft.Azure.Functions.Worker
dotnet add package Microsoft.Azure.Functions.Worker.Extensions.Http
```

**`Program.cs` – DI registration**

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddMcpServer()
            .WithToolsFromAssembly()
            .WithResourcesFromAssembly()
            .WithPromptsFromAssembly();
    })
    .Build();

await host.RunAsync();
```

<!--
- Zeit: 2 min
- Azure Functions isolated worker ist das moderne Modell (.NET 8 / .NET 10) – nicht den älteren in-process-Host verwenden.
- Die Tool-/Resource-/Prompt-Klassen bleiben identisch zu stdio und ASP.NET Core – nur der Host-Wrapper ändert sich.
- `McpServerHttpHandler` ist die abstrakte Komponente, die HTTP-Request/-Response in das MCP-Protokoll übersetzt. Exakter API-Name kann je nach SDK-Version variieren – vor dem Event prüfen.
- Stateless-Transport-Hinweis: Azure Functions sind zustandslos – das passt direkt zu `Stateless = true` im MCP HTTP-Transport.
- Produktionsrelevanz: Viele Enterprise-Teams, die bereits Functions nutzen, können so MCP-Fähigkeiten mit minimaler Infrastruktur exposieren.
- Auth: Function-Keys für einfache Szenarien, Azure AD für Unternehmensumgebungen.
-->

---
zoom: 0.95
---

# Current Developments: MCP Auto-Discovery

### `/.well-known/mcp.json` — SEP-1649 (decentralised)

```json
{
    "mcpServers": {
        "star-agent": {
            "url": "https://yourdomain.com/mcp",
            "name": "StarAgent",
            "description": "AI tour manager for concerts and artists"
        }
    }
}
```

### `/.well-known/mcp/server.json` — Registry approach (centralised)

```json
{
    "name": "StarAgent",
    "description": "AI tour manager for concerts and artists",
    "url": "https://yourdomain.com/mcp",
    "categories": [
        "entertainment",
        "events"
    ]
}
```

<!--
- Zeit: 2 min
- Einordnung: Das ist Stand 2025/2026 – aktiv in Entwicklung, noch nicht überall implementiert.
- SEP-1649-Analogie: Kennt ihr `robots.txt`? Ein Agent besucht eine Website und schaut nach `/.well-known/mcp.json` – und findet damit automatisch alle MCP-Fähigkeiten dieser Domain. Kein zentrales Verzeichnis nötig.
- Registry-Analogie: Wie der App Store. Der MCP-Server meldet sich einmal an, und alle Clients können ihn über die Registry finden.
- Warum beide? SEP-1649 ist perfekt für autonome Agenten, die im Web unterwegs sind. Die Registry ist perfekt für kuratierte, vertrauenswürdige Verzeichnisse in Enterprise-Umgebungen.
-->

---

# Discovery models in parallel

```mermaid
flowchart LR
    A["AI Agent"] -->|" visits URL "| B["/.well-known/mcp.json<br/>(SEP-1649)"]
    A -->|" searches "| C["Central Registry<br/>(Anthropic / GitHub / MS)"]
    B -->|" decentralised<br/>zero-config "| D["MCP Server"]
    C -->|" curated<br/>App-Store "| D
```

<!--
- Zeit: 1 min
- Praxishinweis: Wer heute einen MCP-Server baut, sollte `/.well-known/mcp.json` schon vorsehen – der Aufwand ist minimal, der Zukunftswert groß.
-->

---

# Key Takeaways

- **MCP standardizes AI-to-system integration** – one protocol, any host, any model
- **Three primitives – three concerns:**
    - `Tool` → execute (dynamic, side effects possible)
    - `Resource` → read (stable URI, read-only)
    - `Prompt` → orchestrate (reusable template)
- **The host is the control layer** – the LLM proposes, the host decides
- **Transport is a deployment decision** – stdio locally, HTTP remotely, Functions serverlessly
- **Auto-discovery is coming** – `/.well-known/mcp.json` and central registries
- **Start small:** one tool · one server · connect to your host

<!--
- Zeit: 2 min
- Kernbotschaften nochmal kurz zusammenfassen – nicht vorlesen, sondern in eigenen Worten.
- Wichtigste Botschaft für Entwickelnde: Es ist weniger Aufwand als gedacht. Attribute drauf, DI fertig, Server läuft.
- Wichtigste Botschaft für Nicht-Entwickelnde: MCP schafft eine klare, auditierbare Grenze zwischen dem LLM und euren Systemen. Das ist gut für Governance und Sicherheit.
-->

---

# Where to go next

| Resource                          | Link                                                         |
|-----------------------------------|--------------------------------------------------------------|
| Official MCP Registry (preview)   | `modelcontextprotocol.io/registry`                           |
| Community MCP Directory (curated) | `mcp.directory/awesome-mcp-servers`                          |
| MCP specification                 | `modelcontextprotocol.io/specification`                      |
| .NET SDK quickstart               | `learn.microsoft.com/dotnet/ai/get-started-mcp`              |
| Build a minimal server            | `learn.microsoft.com/dotnet/ai/quickstarts/build-mcp-server` |
| NuGet package                     | `nuget.org/packages/ModelContextProtocol`                    |

<!--
- Zeit: 1 min
- Q&A öffnen.
-->

---
hideInToc: true
layout: cover
background: /assets/BLMeetingBackground.png
---

# Thank you!

<img src="/assets/QR.png" alt="QR code" class="absolute right-16 top-16 w-40">

<!--
- Zeit: 1 min
- Demo-Repo oder Slides-Link zum Nachschlagen kommunizieren.
-->

---
layout: terminal
cast: /assets/casts/sw.cast
---