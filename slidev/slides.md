---
theme: default
title: "Behind the Scenes: MCP"
favicon: /favicon.svg
author: "Denis Sowa"
audience: all
colorSchema: light
themeConfig:
    primary: "#216ec7"
fonts:
    sans: Fira Sans
    serif: Cambria
info: |
    Business Line Meeting · "The Show must go on!" · 60 min · EN slides / DE spoken
drawings:
    persist: false
    presenterOnly: true
addons:
    - slidev-addon-timing-bar
    - slidev-addon-animated-text
transition: fade
duration: 60min
timer: countdown
wakeLock: false
routerMode: hash
layout: cover
hideInToc: true
class: retro-tv-vcr
background: "/assets/BLMeetingBackground.png"
section: { title: Welcome, duration: 3m }
---

# Business<br/>Line Meeting

_The show must go on.<br/>
Oberhausen. 2026._

---
layout: cover
hideInToc: true
transition: slide-left
background: "assets/BLMeetingBackground.png"
---

# Business<br/>Line Meeting

_The show must go on.<br/>
Oberhausen. 2026._

---
layout: intro
transition: slide-left
hideInToc: true
---

# Behind the Scenes: MCP

The Director Between AI and Enterprise Data

<!--
- Kurz die Energie des Event-Themas aufgreifen: Heute sind wir alle Stars – und **StarAgent** managed die Tour.
- Erwartung setzen: kein reiner Theorie-Vortrag. Am Ende läuft ein echter MCP-Server live.
- Ziel benennen: Jede Person hier soll danach in der Lage sein, den Kommunikationsfluss zwischen LLM und MCP zu erklären – und wissen, wie sie selbst einen Server bauen kann.
-->

---
layout: section
transition: slide-left
background: "/assets/SectionBackground.png"
hideInToc: true
---

# Denis Sowa

Architect, AI-Ambassador<br/>
BL Microsoft, Hannover


---
layout: center
transition: slide-left
class: toc
hideInToc: true
---

# _Today's Setlist_

<Toc :columns="2" :maxDepth="1" />

<!--
- Die Präsentation gliedert sich in drei Teile:
  - Was ist "MCP"
  - Wie funktioniert "MCP"
  - Wir implementieren "MCP"
  - Ausblick
- Auf GitHub gibt es zusätzliche Folien zur Verteifung.
-->

---
transition: slide-up
section: { title: "Why MCP Matters", duration: 6m }
---

# Why MCP Matters

### The world before MCP

| Problem                                   | Reality                        |
|-------------------------------------------|--------------------------------|
| Every AI integration was custom-built     | Glue code everywhere           |
| Connectors not portable across hosts      | Rewrite for each AI app        |
| No standard for security or lifecycle     | Every team reinvents the wheel |
| Maintenance cost grew with each new model | Fragile, tightly coupled       |

> MCP standardizes the contract between AI and the outside world.

<!--
- Pain Points aus dem eigenen Umfeld nennen: Wer hat schon mal einen eigenen Connector für ein LLM gebaut?
- Analogie: Vor USB gab es für jedes Gerät einen anderen Stecker. MCP ist der USB-Standard für AI-Integrationen.
- Herkunft: MCP wurde von **Anthropic** entwickelt und im **November 2024** als offener Standard veröffentlicht. Seitdem wird es von Microsoft, GitHub, Google und zahlreichen anderen Unternehmen aktiv unterstützt und weiterentwickelt.
- Kernbotschaft: MCP ist kein Framework, kein Produkt – es ist ein offenes Protokoll, das den Vertrag zwischen AI-Host und externer Fähigkeit definiert.
-->

---
transition: slide-left
zoom: 0.95
---

# Useful MCP Servers

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
- Hinweis: Die Verzeichnis-URLs sind später auf der Folie "Where to go next" aufgeführt.
- **Überleitung:** was genau ist dieses Protokoll?
-->

---
transition: slide-left
---

# What Is MCP

### Model Context Protocol

- **Wire protocol:** JSON-RPC 2.0 – both sides speak structured text
- **Capability model:** Tools · Resources · Prompts
    - *Tasks, MCP Apps, Elicitation, Structured Output, Resource Links, OAuth 2.1, Sampling, Streamable HTTP*
- **Lifecycle management:** connection setup, capability discovery, invocation, teardown
- **Interoperability:** one server works with any MCP-compatible host
- **Transports:** `stdio` for local processes · `Streamable HTTP` for remote services

> An open standard that defines how AI applications securely and structurally connect to tools and data sources.

<!--
- MCP als Protokoll einordnen, nicht als Bibliothek oder Framework.
- JSON-RPC 2.0 hervorheben: Host und Server tauschen schlicht strukturierten Text aus – dazu gleich mehr.
- Neue Features:
    - *Tasks (asynchrone Ausführung)*
    - ~~MCP Apps (UI-Rendering)~~
    - Elicitation (Server fragt User)
        - Aktuell nur VS Code / GitHub Copilot
    - Structured Output
    - Resource Links
    - OAuth 2.1 / Authorization
    - Sampling (Server → LLM)
    - Streamable HTTP / SSE-Resumability (Client reconnect)
    
- Transport kurz erwähnen: lokal läuft es über stdio (Standard-Ein-/Ausgabe), remote über HTTP. Details kommen im Architektur-Diagramm.
- Interoperabilität betonen: ein MCP-Server in .NET funktioniert mit GitHub Copilot, Claude Desktop, VS Code und jedem anderen MCP-Host.
-->

---
transition: fade
section: { title: Architecture, duration: 5m }
---

# MCP Architecture

```mermaid {scale: 0.65}
flowchart LR
    subgraph YC["Your Machine"]
        direction LR
        H["Host<br/>(IDE / Agent Shell)"]
        C["MCP Client"]
        S1["MCP Server A<br/>(local · stdio)"]
        DS1[("Local<br/>Data Source")]
        H <--> C
        C <-->|" JSON-RPC 2.0<br/>(stdio) "| S1
        S1 <--> DS1
    end

```

<!--
- Die drei Rollen klar abgrenzen:
  - Host = AI-App
  - Client = Protokollschicht im Host
  - Server = Fähigkeiten-Anbieter
- Wichtiger Punkt: Host und Server können unabhängig voneinander entwickelt werden – das ist die Stärke des Standards.
- Der Host kann mehrere Clients gleichzeitig nutzen.
- Diagramm erläutern: lokal über stdio (einfach, schnell, für Entwicklung), remote über HTTP (produktionstauglich, skalierbar).
- Beispiel aus der Praxis: VS Code mit GitHub Copilot ist der Host + Client. Unser StarAgent-Server ist der MCP Server.
-->

---
transition: fade
hideInToc: true
---

# Architecture: Multiple Servers

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
    subgraph Internet["External Systems"]
        DS2[("Database /<br/>File Store")]
    end
    S2 <-->|" Web APIs "| DS2
```

<!--
- Die drei Rollen klar abgrenzen:
  - Host = AI-App
  - Client = Protokollschicht im Host
  - Server = Fähigkeiten-Anbieter
- Wichtiger Punkt: Host und Server können unabhängig voneinander entwickelt werden – das ist die Stärke des Standards.
- Der Host kann mehrere Clients gleichzeitig nutzen.
- Diagramm erläutern: lokal über stdio (einfach, schnell, für Entwicklung), remote über HTTP (produktionstauglich, skalierbar).
- Beispiel aus der Praxis: VS Code mit GitHub Copilot ist der Host + Client. Unser StarAgent-Server ist der MCP Server.
-->

---
transition: slide-up
hideInToc: true
---

# Architecture: Local + Remote

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
- Die drei Rollen klar abgrenzen:
  - Host = AI-App
  - Client = Protokollschicht im Host
  - Server = Fähigkeiten-Anbieter
- Wichtiger Punkt: Host und Server können unabhängig voneinander entwickelt werden – das ist die Stärke des Standards.
- Der Host kann mehrere Clients gleichzeitig nutzen.
- Diagramm erläutern: lokal über stdio (einfach, schnell, für Entwicklung), remote über HTTP (produktionstauglich, skalierbar).
- Beispiel aus der Praxis: VS Code mit GitHub Copilot ist der Host + Client. Unser StarAgent-Server ist der MCP Server.
-->

---
transition: slide-left
hideInToc: true
---

# Architecture: Roles

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
- Die drei Rollen klar abgrenzen:
  - Host = AI-App
  - Client = Protokollschicht im Host
  - Server = Fähigkeiten-Anbieter
-->

---
transition: slide-up
section: { title: Capabilities, duration: 3m }
---

# Capabilities

### Three primitives – three distinct concerns

| Primitive    | Purpose                                            | StarAgent example                                               |
|--------------|----------------------------------------------------|-----------------------------------------------------------------|
| **Tool**     | Executable function · model calls it, host runs it | `get_chart_position` – where is a song on the charts?           |
| **Resource** | Read-only data, addressable by URI                 | `rider://artist/van-halen` – Van Halen's backstage requirements |
| **Prompt**   | Reusable prompt template with placeholders         | `concert_press_release` – generate a dramatic tour announcement |

<!--
- Die Semantik der drei Primitive präzise machen:
  - Tool = execute
  - Resource = read
  - Prompt = orchestrate
- StarAgent-Beispiele direkt zeigen: Wir bauen gleich alle drei live.
- Rider erklären: Ein Rider ist das echte Dokument, das jeder Künstler vor einem Konzert einreicht – Bühnenanforderungen, Catering, Sonderwünsche. Van Halens berühmteste Forderung: „Absolutely NO brown M&Ms.“ Das ist ein perfektes Beispiel für eine Resource – stabil, adressierbar, read-only.
- Enterprise-Brücke: Statt Rider → euer Konfigurations-Dokument, euer OpenAPI-Spec, euer Feature-Spec. Das Prinzip ist identisch.
-->

---
transition: slide-left
hideInToc: true
---

# Capabilities: Decision Guide

- **Tool** → the model needs to _do_ something or fetch dynamic data
- **Resource** → stable, readable document or data set (like a file or config)
- **Prompt** → standardized, repeatable workflow the model should follow

<!--
- Governance-Hinweis: Die drei Primitiv-Typen haben unterschiedliche Risikoprofile – Tools können Seiteneffekte haben, Resources und Prompts sind read-only.
-->

---
transition: slide-up
section: { title: Runtime, duration: 3m }
---

# Runtime: Host as Translator

### The LLM never sees MCP

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
- Kernbotschaft deutlich machen: LLM und MCP-Server sprechen **nie direkt** miteinander. Der Host ist immer der Vermittler und die Kontrollinstanz.
- JSON-RPC 2.0 ist kein Hexenwerk – es sind strukturierte Textnachrichten mit `method`, `params` und `result`.
- Auf Git folgen weitere Folien, mit detalierteren Beschreibungen.
- Dann kommen wir zu: **Wir implementieren "MCP"**
-->

---
zoom: 0.75
transition: slide-up
hideFor: live
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
- Discovery-Vorgang: Der Host fragt den Server nach seinen Fähigkeiten und übersetzt das in Function-Definitions für das Modell.
- Highlight: Das LLM „sieht“ nur die Tool-Schemata – es weiß nicht, ob dahinter .NET, Python oder ein Toaster steckt.
-->

---
zoom: 0.85
transition: slide-up
hideFor: live
hideInToc: true
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
- Highlight: Das LLM „sieht“ nur die Tool-Schemata – es weiß nicht, ob dahinter .NET, Python oder ein Toaster steckt.
-->

---
transition: slide-up
hideFor: live
hideInToc: true
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
- Das LLM entscheidet, ob es ein Tool aufrufen will – es gibt einfach JSON zurück. Keine Magie.
-->

---
transition: slide-up
hideFor: live
hideInToc: true
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
- Host führt den Tool-Call aus (policy check, ggf. user approval) und schickt das Ergebnis als neuen Context an das Modell.
-->

---
transition: slide-left
hideFor: live
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
- Host führt den Tool-Call aus (policy check, ggf. user approval) und schickt das Ergebnis als neuen Context an das Modell.
-->

---
transition: slide-up
---

# Runtime Sequence

### End-to-end flow

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
- Lifecycle-Tabelle nur kurz streifen: Diese Calls gibt es, der Host verwaltet sie automatisch. Man muss sie nicht selbst implementieren – **das SDK erledigt das**.
- Sequenzdiagramm ist das Herzstück: Hier wird sichtbar, dass der Host die Kontrolle behält. Das LLM macht einen Vorschlag (Tool Call), aber der Host entscheidet, ob er ausgeführt wird.
- Wichtige Botschaft: Der Host ist die Sicherheitsinstanz – nicht das LLM.
-->

---
transition: slide-left
hideFor: live
hideInToc: true
---

# Lifecycle Calls

### For reference

| Phase      | Call                                             | Direction       |
|------------|--------------------------------------------------|-----------------|
| Setup      | `initialize` + `notifications/initialized`       | Client → Server |
| Discovery  | `tools/list` · `resources/list` · `prompts/list` | Client → Server |
| Invocation | `tools/call` · `resources/read` · `prompts/get`  | Client → Server |

---
layout: section
transition: slide-left
background: "/assets/SectionBackground.png"
hideInToc: true
---

# Implementation

---
transition: slide-left
section: { title: Implementation, duration: 12m }
---

# .NET SDK

### Getting started

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

<!--
- SDK einordnen: War gerade noch Preview-Paket. Jetzt Version 1.4.
- **Überleitung:** Genug geredet, wir erstellen das StarAgent Projekt.
-->

---
title: "Create Project"
transition: slide-left
layout: terminal
---

<Asciinema src="/assets/casts/createproject.cast"/>

---
layout: codeeditor
transition: none
---

> // Program.cs
<<< @/snippets/Program.cs

<!--
- Program.cs zeigen und auf den nächsten Folien erklären.
-->

---
layout: codeeditor
transition: none
hideInToc: true
---

> // Program.cs
<<< @/snippets/Program.cs {1-5}

---
layout: codeeditor
transition: none
hideInToc: true
---

> // Program.cs
<<< @/snippets/Program.cs {7-8}

<!--
- Logging-Hinweis: Bei stdio läuft die Protokollkommunikation über stdout. Logging immer auf stderr oder in eine Datei umleiten, damit keine Lognachrichten das Protokoll stören.
-->

---
layout: codeeditor
transition: none
hideInToc: true
---

> // Program.cs
<<< @/snippets/Program.cs {10-11}

---
layout: codeeditor
transition: none
hideInToc: true
---

> // Program.cs
<<< @/snippets/Program.cs {12}

---
layout: codeeditor
transition: none
hideInToc: true
---

> // Program.cs
<<< @/snippets/Program.cs {13}

<!--
- Hier mit Stdio.
-->

---
layout: codeeditor
transition: none
hideInToc: true
---

> // Program.cs
<<< @/snippets/Program.cs {14-16}

<!--
- `WithToolsFromAssembly()` / `WithResourcesFromAssembly()` / `WithPromptsFromAssembly()` – alle Klassen mit den entsprechenden Attributen im Assembly werden automatisch registriert.
- `.WithToolsFromAssembly(typeof(ChartTools).Assembly)`
- `.WithTools<Tool>()` – nur eine bestimmte Klasse registrieren.
-->

---
layout: codeeditor
transition: slide-left
hideInToc: true
---

> // Program.cs
<<< @/snippets/Program.cs {18}

<!--
- **Überleitung:** Was implementieren wir in der Demo
-->

---
transition: slide-left
---

### Demo capabilities

- Tool: `get_chart_position, book_venue`
- Resource: `rider://artist/{name}`
- Prompt: `concert_press_release`

<!--
- Drei Klassen nacheinander implementieren.
- Punchline zum Diagramm: Das letzte Ergebnis – „Bohemian Rhapsody ist #1 – wie immer“ – ist unser Mock-Verhalten für die Demo. Kommt gleich live.
-->

---
hideInToc: true
transition: slide-left
layout: terminal
---

<Asciinema src="/assets/casts/projectstructure.cast"/>

<!--
- Kurz die Magie der Demo erklären: Ich habe da schonmal etwas vorbereitet…
- **Überleitung:** Schauen wir uns die Projektstruktur an – drei Klassen für Tools, Resources und Prompts. Alle sind komplett leer – wir füllen sie gleich live.
-->

---
layout: codeeditor
transition: slide-left
showFor: live
---

> // ChartTools.cs

<MonacoSync />
```csharp {monaco}  {height:'460px'}
using ModelContextProtocol.Server;
using StarAgent.McpServer.Shared.Models;
using StarAgent.McpServer.Shared.Services;
using System.ComponentModel;

[McpServerToolType]
public static class ChartTools
{

}

```

<!--
```

    [McpServerTool(Name = "get_chart_position")]
    [Description("Returns the chart position of a song on a given chart.")]
    public static ChartResult GetChartPosition(
        [Description("Song title")] string songTitle,
        [Description("Artist name")] string artist,
        [Description("Chart name")] string chart = "Billboard Hot 100")
    {
        return ChartDataService.Lookup(songTitle, artist, chart);
    }

```

- Attribute-Ansatz betonen: Wer .NET kennt, fühlt sich sofort zu Hause. Kein Boilerplate, kein manuelles JSON-Parsing.
- Description-Attribute sind entscheidend: Sie landen direkt im Tool-Schema, das das LLM sieht. Je klarer die Description, desto besser die Tool-Auswahl durch das Modell.
-->

---
layout: codeeditor
transition: slide-left
hideFor: live
---

> // ChartTools.cs

<CodeBlockSync />
<<< @/snippets/ChartTools.cs {maxHeight: '440px'}

---
layout: codeeditor
transition: slide-left
showFor: live
---

> // RiderResources.cs

<MonacoSync />
```csharp {monaco}  {height:'460px'}
using ModelContextProtocol.Server;
using StarAgent.McpServer.Shared.Models;
using StarAgent.McpServer.Shared.Services;
using System.ComponentModel;

[McpServerResourceType]
public static class RiderResources
{

}

```

<!--
```
    [McpServerResource(
        UriTemplate = "rider://artist/{name}",
        Name = "artist_rider",
        MimeType = "application/json")]
    [Description("Returns the backstage rider for an artist.")]
    public static string GetRider(
        [Description("Artist slug")] string name)
    {
        return RiderDataService.Load(name);
    }
```

- **Hinweis:** Der Host kann auf ein Update der Resource abonieren und wird dann entsprechend benachrichtigt.
-->

---
layout: codeeditor
transition: slide-left
hideFor: live
---

> // RiderResources.cs

<<< @/snippets/RiderResources.cs {maxHeight: '440px'}

---
layout: codeeditor
transition: slide-left
---

> // McpServerPromptType.cs

<CodeBlockSync />
<<< @/snippets/PressReleasePrompts.cs {maxHeight: '440px'}

---
title: Register MCP
transition: slide-left
---

# Register: stdio

### Development with stdio transport

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
- Jetzt den MCP in der Konfiguration hinzufügen.
- ClaudeCode erkennt den MCP automatisch (.mcp/server.json).
-->

---
layout: center
transition: none
section: { title: Demo, duration: 5m }
---

# Demo

<!--
- **Überleitung:** Wir nutzen den MCP-Server.
- Claude erkennt MCP automatisch
-->

---
transition: slide-left
layout: terminal
hide: true
---

<Asciinema src="/assets/casts/mcp.cast" />

<!--
- Rider-Punchline: Van Halen öffnen → „Absolutely NO brown M&Ms" live im Chat auftauchen lassen. 🤘
- Wo steht Bohemian Rhapsody von Queen in den Charts?
- Zeig mir das Backstage-Rider für Van Halen
    - Hier gibt es diverse Probleme. Ist aber für die Demo nicht schlecht.
- /mcp__StarAgent__concert_press_release Queen "Metronom Theater Oberhausen" 26.06.2026 "The show must go on!"
-->

---
transition: slide-left
---

# Debug-Tipp

> npx @modelcontextprotocol/inspector

<img src="@/public/assets/Inspector.png" alt="MCP Inspector" class = "h-100">

<!--
- Features:
    - Resources
    - Prompts
    - Tools
- Neu:
    - Tasks
    - Apps
    - Usw.

Zu einigen Punkten (z.B. Apps) wird Lukas Beerschwinger noch einen Talk in der Brownbacksession halten.
-->

---
transition: slide-left
---

# Host-side: discover and invoke

```csharp
await using var client = await McpClient.CreateAsync(transport);

var tools  = await client.ListToolsAsync();
var result = await client.CallToolAsync("get_chart_position",
    new { songTitle = "Bohemian Rhapsody", artist = "Queen" });
```

<!--
- Das SDK kann MCP-Server auch nutzen: Host-Seite zeigen: ListToolsAsync gibt die Discovery zurück, CallToolAsync führt aus. Genau das, was wir als JSON-RPC gesehen haben – jetzt als typisierter .NET-Aufruf.
-->

---
transition: slide-left
section: { title: "Deployment Options", duration: 3m }
---

# HTTP Transport: Reference

### From stdio to Streamable HTTP – minimal changes

```shell
dotnet add package ModelContextProtocol.AspNetCore
```

> // Program.cs

```csharp {monaco-diff}  {height:'270px'}
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

<!--
- Folie ist reine Referenz – kein Live-Coding hier.
- Hauptaussage: Die Tool-/Resource-/Prompt-Implementierungen bleiben 1:1 identisch. Nur `Program.cs` ändert sich.
- Auth-Hinweis: Bei HTTP-Transport ist Authentifizierung Pflicht in Produktion. Typisch: Azure AD, API-Keys oder OAuth.
-->

---
transition: slide-left
---

# Register: HTTP

### Development with HTTP transport

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

---
transition: slide-up
---

# Azure Function: Reference

### Serverless hosting

> // Program.cs

```csharp
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

FunctionsApplicationBuilder builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
       .AddApplicationInsightsTelemetryWorkerService()
       .ConfigureFunctionsApplicationInsights();

await builder.Build().RunAsync();

```

<!--
- Azure Functions isolated worker ist das moderne Modell (.NET 8 / .NET 10) – nicht den älteren in-process-Host verwenden.
- Die Tool-/Resource-/Prompt-Klassen bleiben identisch zu stdio und ASP.NET Core – nur der Host-Wrapper ändert sich.
- `McpServerHttpHandler` ist die abstrakte Komponente, die HTTP-Request/-Response in das MCP-Protokoll übersetzt. Exakter API-Name kann je nach SDK-Version variieren – vor dem Event prüfen.
- Stateless-Transport-Hinweis: Azure Functions sind zustandslos. Damit ist kein Sampling möglich.
- Produktionsrelevanz: Viele Enterprise-Teams, die bereits Functions nutzen, können so MCP-Fähigkeiten mit minimaler Infrastruktur exposieren.
- Auth: Function-Keys für einfache Szenarien, Azure AD für Unternehmensumgebungen.
-->

---
transition: slide-left
hideInToc: true
---

# Azure Function

> // McpToolFunctions.cs

```csharp
[Function(nameof(GetChartPosition))]
public ChartResult GetChartPosition([McpToolTrigger("get_chart_position", "Returns the chart position of a song on a given chart.")] ToolInvocationContext context,
                                    [McpToolProperty("songTitle", "Song title", true)] string songTitle,
                                    [McpToolProperty("artist", "Artist name", true)] string artist,
                                    [McpToolProperty("chart", "Chart name")] string? chart)
{
    string resolvedChart = string.IsNullOrWhiteSpace(chart)
        ? "Billboard Hot 100"
        : chart;

    return ChartDataService.Lookup(songTitle, artist, resolvedChart);
}
```

---
layout: section
transition: slide-left
background: "/assets/SectionBackground.png"
hideInToc: true
---

# Outlook

---
transition: slide-up
zoom: 0.95
section: { title: Outlook, duration: 5m }
---

# Auto-Discovery

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
- Einordnung: Das ist Stand 2025/2026 – aktiv in Entwicklung, noch nicht überall implementiert.
- SEP-1649-Analogie: Kennt ihr `robots.txt`? Ein Agent besucht eine Website und schaut nach `/.well-known/mcp.json` – und findet damit automatisch alle MCP-Fähigkeiten dieser Domain. Kein zentrales Verzeichnis nötig.
- Registry-Analogie: Wie der App Store. Der MCP-Server meldet sich einmal an, und alle Clients können ihn über die Registry finden.
- Warum beide? SEP-1649 ist perfekt für autonome Agenten, die im Web unterwegs sind. Die Registry ist perfekt für kuratierte, vertrauenswürdige Verzeichnisse in Enterprise-Umgebungen.
-->

---
transition: slide-left
---

# Discovery Models

```mermaid
flowchart LR
    A["AI Agent"] -->|" visits URL "| B["/.well-known/mcp.json<br/>(SEP-1649)"]
    A -->|" searches "| C["Central Registry<br/>(Anthropic / GitHub / MS)"]
    B -->|" decentralised<br/>zero-config "| D["MCP Server"]
    C -->|" curated<br/>App-Store "| D
```

<!--
- Praxishinweis: Wer heute einen MCP-Server baut, sollte `/.well-known/mcp.json` schon vorsehen – der Aufwand ist minimal, der Zukunftswert groß.
-->

---
transition: slide-left
---

# Key Takeaways

- **MCP standardizes AI-to-system integration** – one protocol, any host, any model
- **Capabilities & features:** `Tool` · `Resource` · `Prompt` · `Tasks` · `Elicitation` · `Structured Output` ·
  `Resource Links` · `OAuth 2.1` · `Sampling` · `Streamable HTTP`
- **The host is the control layer** – the LLM proposes, the host decides
- **Transport is a deployment decision** – stdio locally, HTTP remotely, Functions serverlessly
- **Auto-discovery is coming** – `/.well-known/mcp.json` and central registries
- **Start small:** one tool · one server · connect to your host

<!--
- Kernbotschaften nochmal kurz zusammenfassen – nicht vorlesen, sondern in eigenen Worten.
- Wichtigste Botschaft für Entwickelnde: Es ist weniger Aufwand als gedacht. Attribute drauf, DI fertig, Server läuft.
- Wichtigste Botschaft für Nichtentwickelnde: MCP schafft eine klare, auditierbare Grenze zwischen dem LLM und euren Systemen. Das ist gut für Governance und Sicherheit.
-->

---
transition: slide-left
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
- **Überleitung:** Q&A öffnen.
-->

---
layout: section
transition: slide-left
background: "/assets/SectionBackground.png"
hideInToc: true
section: { title: "Q&A", duration: 14m }
---

# Q&A

---
transition: slide-up
hideInToc: true
layout: cover
background: "/assets/BLMeetingBackground.png"
section: { title: "Bye", duration: 1m }
---

<animated-text text-8xl text-primary text="Thank you!" />

<img src="@/assets/QR.svg" alt="https://github.com/L-C-P/mcp-talk-dotnet-demo" class="absolute right-16 top-16 w-40">

<!--
- Demo-Repo oder Slides-Link zum Nachschlagen kommunizieren.
-->

---
zoom: 1.7
layout: terminal
hideFor: live
---

<Asciinema src="/assets/casts/sw.cast"/>
