# Slide 04 – MCP Architecture

## Slide text (EN)

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

```mermaid
flowchart LR
    subgraph YC["Your Machine"]
        H["Host<br/>(IDE / Agent Shell)"]
        C["MCP Client"]
        S1["MCP Server A<br/>(local · stdio)"]
        S2["MCP Server B<br/>(local · stdio)"]
        DS1[("Local<br/>Data Source")]
        H <--> C
        C <-->|"JSON-RPC 2.0<br/>(stdio)"| S1
        C <-->|"JSON-RPC 2.0<br/>(stdio)"| S2
        S1 <--> DS1
    end
    subgraph Remote["Remote"]
        RS["MCP Server C<br/>(remote · HTTP)"]
    end
    subgraph Internet["External Systems"]
        DS2[("Database /<br/>File Store")]
        RSVC[("Remote<br/>Service / API")]
    end
    S2 <-->|"Web APIs"| DS2
    C <-->|"JSON-RPC 2.0<br/>(Streamable HTTP)"| RS
    RS <-->|"Web APIs"| RSVC
```

> One host can connect to multiple MCP servers simultaneously.

---

## Speaker notes (DE)

- Die drei Rollen klar abgrenzen: Host = AI-App, Client = Protokollschicht im Host, Server = Fähigkeiten-Anbieter.
- Wichtiger Punkt: Host und Server können unabhängig voneinander entwickelt werden – das ist die Stärke des Standards.
- Diagramm erläutern: links lokal über stdio (einfach, schnell, für Entwicklung), rechts remote über HTTP (produktionstauglich, skalierbar).
- Beispiel aus der Praxis: VS Code mit GitHub Copilot ist der Host + Client. Unser StarAgent-Server ist der MCP Server.
