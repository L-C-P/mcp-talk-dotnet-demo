# MCP verstehen, MCP-Server bauen: Die Brücke zwischen LLMs und Unternehmensdaten

## 0) Einreichungsunterlagen

### Titelvorschlag

Behind the Scenes: MCP – The Director Between AI and Enterprise Data

### Abstract

Model Context Protocol (MCP) ist der offene Standard, mit dem KI-Anwendungen sicher und strukturiert auf
Unternehmensdaten und bestehende Systeme zugreifen. Der Vortrag zeigt zuerst verständlich, warum MCP entstanden ist und
wie Host, Client und Server zusammenspielen. Danach folgt eine konkrete .NET-Perspektive: Capability-Design (Tools,
Resources, Prompts), Discovery, Runtime-Flow, stdio vs Streamable HTTP und ein Live-Demo-Server. Zum Abschluss werden
Azure-Functions-Hosting und aktuelle Entwicklungen wie MCP Auto-Discovery eingeordnet.

### Bewerbungstext

Der Talk richtet sich an Tech und Business gleichermaßen: Entwickelnde erhalten einen klaren Implementierungspfad mit
dem Microsoft MCP SDK, Architektur- und Governance-Rollen bekommen ein belastbares Modell für Sicherheitsgrenzen und
Integrationsverantwortung. Die Session beantwortet die typische Praxisfrage aus vielen AI-Initiativen: Wie kann ein LLM
kontrolliert mit echten Unternehmenssystemen interagieren, ohne pro Use Case neue Spezial-Connectoren zu bauen?

## 1) Session profile

- Format: Frontal presentation with one live demo
- Total duration: 60 minutes
- Spoken language: German
- Slide language: English
- Audience: .NET developers, software architects, tech leads, product owners, project managers, and AI decision-makers
- Goal: Deliver a practical, technically correct MCP overview and show how to build and expose a .NET MCP server

## 2) Learning outcomes

By the end of the talk, the audience should be able to:

- Explain why MCP exists and which integration problems it solves.
- Distinguish Tools, Resources, and Prompts including their governance/risk differences.
- Describe the host-centered communication model (host as translator and control layer).
- Read the essential MCP lifecycle/runtime calls (`initialize`, `tools/list`, `tools/call`, etc.).
- Choose between stdio, Streamable HTTP, and serverless hosting patterns.
- Bootstrap a minimal MCP server in .NET and connect it to an MCP host.

## 3) 45-minute agenda (aligned to current Slidev deck)

- 00:00-02:00 Opening: Behind the Scenes framing and objective
- 02:00-04:00 Why We Needed MCP
- 04:00-06:00 MCP servers already in use today
- 06:00-08:00 What Is MCP?
- 08:00-10:00 MCP Architecture
- 10:00-13:00 Capabilities: Tools, Resources, Prompts
- 13:00-15:00 How LLM and MCP Actually Talk (host translation model)
- 15:00-18:00 It's just text (1/2): discovery and schema translation
- 18:00-20:00 It's just text (2/2): tool call and result loop
- 20:00-22:00 Discovery & Runtime Sequence
- 22:00-24:00 Microsoft MCP SDK for .NET (1/2): project setup
- 24:00-26:00 Microsoft MCP SDK for .NET (2/2): attributes and host calls
- 26:00-34:00 Live Demo: StarAgent MCP Server (stdio)
- 34:00-36:00 HTTP Transport: Reference setup
- 36:00-38:00 MCP on Azure Functions
- 38:00-40:00 Current Developments: MCP Auto-Discovery
- 40:00-41:00 Discovery models in parallel (decentralized vs registry)
- 41:00-43:00 Key Takeaways
- 43:00-45:00 Where to go next + Q&A

## 4) Slide storyboard (English slide content, German speaking guidance)

## Slide 1 (Opening): "Behind the Scenes: MCP"

### Slide text (EN)

- Event framing and tagline
- "The Director Between AI and Enterprise Data"

### Speaker guidance (DE)

- Einstieg über Event-Metapher und Zielbild.
- Erwartung setzen: praxisnah, nicht nur Theorie.

## Slide 2: "About me"

### Slide text (EN)

- Speaker identity and role context

### Speaker guidance (DE)

- Kurz halten, direkt zur Relevanz von MCP überleiten.

## Slide 3: "Today's Setlist"

### Slide text (EN)

- 16-part session structure

### Speaker guidance (DE)

- Ablauf transparent machen, Demo früh ankündigen.

## Slide 4: "Why We Needed MCP"

### Slide text (EN)

- Custom integrations, portability issues, high maintenance
- MCP as a standard contract

### Speaker guidance (DE)

- Team-Pain-Points konkret benennen.
- USB-Analogie für Standardisierung nutzen.

## Slide 5: "MCP servers already in use today"

### Slide text (EN)

- Market reality: docs, repos, browsers, databases, ALM tools

### Speaker guidance (DE)

- MCP als etabliertes Ökosystem darstellen, nicht als Zukunftsmusik.

## Slide 6: "What Is MCP?"

### Slide text (EN)

- Open standard, JSON-RPC 2.0, capability model, lifecycle, transports

### Speaker guidance (DE)

- Klarstellen: Protokoll statt Produkt.
- Interoperabilität als Kernnutzen hervorheben.

## Slide 7: "MCP Architecture"

### Slide text (EN)

- Host, MCP client, MCP server roles
- Local stdio and remote HTTP topology

### Speaker guidance (DE)

- Verantwortlichkeiten sauber trennen.
- Host als zentrale Kontrollinstanz erklären.

## Slide 8: "Capabilities: Tools, Resources, Prompts"

### Slide text (EN)

- Three primitives, three concerns
- StarAgent examples (`get_chart_position`, `book_venue`, rider resource, press prompt)

### Speaker guidance (DE)

- Semantik präzise machen: execute vs read vs orchestrate.
- Governance-Unterschiede hervorheben (insb. Side Effects bei Tools).

## Slide 9: "How LLM and MCP Actually Talk"

### Slide text (EN)

- The host translates MCP capabilities to model-native tool/function format

### Speaker guidance (DE)

- Wichtigster Punkt: LLM und MCP-Server sprechen nicht direkt miteinander.

## Slide 10: "It's just text – structured text (1/2)"

### Slide text (EN)

- JSON-RPC `tools/list` exchange
- Function schema injected into model context

### Speaker guidance (DE)

- Discovery und Schema-Mapping Schritt für Schritt erklären.

## Slide 11: "It's just text – structured text (2/2)"

### Slide text (EN)

- Tool call proposal by model
- Host executes `tools/call` and returns results

### Speaker guidance (DE)

- Policy Check und optionale Freigabe im Host betonen.

## Slide 12: "Discovery & Runtime Sequence"

### Slide text (EN)

- Lifecycle reference table
- End-to-end sequence diagram

### Speaker guidance (DE)

- Sequenzdiagramm als Herzstück der Kontrolllogik nutzen.

## Slide 13: "Microsoft MCP SDK for .NET (1/2)"

### Slide text (EN)

- Template, Visual Studio template, package install
- `mcp.json` server registration

### Speaker guidance (DE)

- Setup-Wege vergleichen und praxisnah einordnen.

## Slide 14: "Microsoft MCP SDK for .NET (2/2)"

### Slide text (EN)

- Attribute-based registration
- Host discovery + invocation API calls

### Speaker guidance (DE)

- Nutzen klar machen: wenig Boilerplate, klare Typisierung.

## Slide 15: "Live Demo: StarAgent MCP Server (stdio)"

### Slide text (EN)

- `Program.cs` bootstrap
- Tools, Resource, Prompt live in one server

### Speaker guidance (DE)

- Live-Reihenfolge diszipliniert halten.
- stdout/stderr Logging-Trennung aktiv erwähnen.

## Slide 16: "HTTP Transport: Reference"

### Slide text (EN)

- ASP.NET Core transport wiring
- `mcp.json` host connection via HTTP endpoint

### Speaker guidance (DE)

- Als Referenzfolie führen, nicht als zweite Hauptdemo.

## Slide 17: "MCP on Azure Functions"

### Slide text (EN)

- Isolated worker setup and DI registration

### Speaker guidance (DE)

- Enterprise-Relevanz hervorheben: serverless Hosting fit.

## Slide 18: "Current Developments: MCP Auto-Discovery"

### Slide text (EN)

- `/.well-known/mcp.json` vs registry metadata models

### Speaker guidance (DE)

- Reifegrad als „in Entwicklung“ markieren.

## Slide 19: "Discovery models in parallel"

### Slide text (EN)

- Decentralized discovery and curated registry path

### Speaker guidance (DE)

- Ein-Minuten-Einordnung mit klarer Handlungsempfehlung.

## Slide 20: "Key Takeaways"

### Slide text (EN)

- Standardization, control layer, primitives, transport decision, discovery trend

### Speaker guidance (DE)

- Kernbotschaften verdichten, keine neuen Inhalte öffnen.

## Slide 21: "Where to go next"

### Slide text (EN)

- Spec, quickstarts, package references

### Speaker guidance (DE)

- Konkrete nächste Schritte für unterschiedliche Rollen nennen.

## Slide 22 (Closing): "Vielen Dank"

### Slide text (EN/DE)

- Thank-you slide with QR code

### Speaker guidance (DE)

- Q&A öffnen, Materialien verweisen.

## 5) Demo concept (aligned to current deck)

## Primary demo: StarAgent on stdio

- Show minimal `Program.cs` with `.AddMcpServer()` + `.WithStdioServerTransport()`.
- Expose all three capability categories:
    - Tool: `get_chart_position`
    - Tool: `book_venue`
    - Resource: `rider://artist/{name}`
    - Prompt: `concert_press_release`
- Demonstrate one discovery step and two runtime interactions (tool call + resource read or prompt use).

## Optional extension (if time permits)

- Show equivalent HTTP transport registration and `mcp.json` client binding.

## Fallback plan

- Prepared screenshots for bootstrapping and result screens.
- Keep one static JSON-RPC pair (`tools/list` and `tools/call`) as backup narrative.

## 6) Frontal delivery tips

- Keep architecture-first explanations, then map to .NET APIs.
- Alternate concept and implementation every few minutes.
- Reuse one running StarAgent example for cognitive continuity.
- Repeat one anchor question: "Who is in control when a tool call happens?"

## 7) Preparation checklist

- Verify .NET SDK/package versions one day before the event.
- Smoke-test stdio demo end-to-end with the intended host.
- Rehearse the exact live path with a strict 8-minute demo timer.
- Prebuild fallback screenshots and local notes.
- Timebox full rehearsal to 42 minutes to keep Q&A buffer.

## 8) Suggested references for deck notes

- MCP specification:
    - https://modelcontextprotocol.io/specification
- Microsoft Learn: Get started with .NET AI and MCP:
    - https://learn.microsoft.com/dotnet/ai/get-started-mcp
- Microsoft Learn: Build a minimal MCP server:
    - https://learn.microsoft.com/dotnet/ai/quickstarts/build-mcp-server
- Microsoft Learn: Build a minimal MCP client:
    - https://learn.microsoft.com/dotnet/ai/quickstarts/build-mcp-client
- NuGet package:
    - https://www.nuget.org/packages/ModelContextProtocol
