# Slide 03 – What Is MCP?

## Slide text (EN)

### Model Context Protocol (MCP)

> An open standard that defines how AI applications securely and structurally connect to tools and data sources.

- **Wire protocol:** JSON-RPC 2.0 – both sides speak structured text
- **Capability model:** Tools · Resources · Prompts (more on the next slide)
- **Lifecycle management:** connection setup, capability discovery, invocation, teardown
- **Interoperability:** one server works with any MCP-compatible host
- **Transports:** `stdio` for local processes · `Streamable HTTP` for remote services

---

## Speaker notes (DE)

- MCP als Protokoll einordnen, nicht als Bibliothek oder Framework.
- JSON-RPC 2.0 hervorheben: Host und Server tauschen schlicht strukturierten Text aus – dazu gleich mehr.
- Transport kurz erwähnen: lokal läuft es über stdio (Standard-Ein-/Ausgabe), remote über HTTP. Details kommen im Architektur-Diagramm.
- Interoperabilität betonen: ein MCP-Server in .NET funktioniert mit GitHub Copilot, Claude Desktop, VS Code und jedem anderen MCP-Host.
