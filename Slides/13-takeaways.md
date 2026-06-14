# Slide 13 – Key Takeaways

## Slide text (EN)

### What to take home

- **MCP standardizes AI-to-system integration** – one protocol, any host, any model
- **Three primitives – three concerns:**
  - `Tool` → execute (dynamic, side effects possible)
  - `Resource` → read (stable URI, read-only)
  - `Prompt` → orchestrate (reusable template)
- **The host is the control layer** – the LLM proposes, the host decides
- **Transport is a deployment decision** – stdio locally, HTTP remotely, Functions serverlessly
- **Auto-discovery is coming** – `/.well-known/mcp.json` and central registries
- **Start small:** one tool · one server · connect to your host

---

### Where to go next

| Resource | Link |
|---|---|
| MCP specification | `modelcontextprotocol.io/specification` |
| .NET SDK quickstart | `learn.microsoft.com/dotnet/ai/get-started-mcp` |
| Build a minimal server | `learn.microsoft.com/dotnet/ai/quickstarts/build-mcp-server` |
| NuGet package | `nuget.org/packages/ModelContextProtocol` |

---

> *"The show must go on – and now your AI has a proper tour manager."* 🎸

---

## Speaker notes (DE)

- Kernbotschaften nochmal kurz zusammenfassen – nicht vorlesen, sondern in eigenen Worten.
- Wichtigste Botschaft für Entwickelnde: Es ist weniger Aufwand als gedacht. Attribute drauf, DI fertig, server läuft.
- Wichtigste Botschaft für Nicht-Entwickelnde: MCP schafft eine klare, auditierbare Grenze zwischen dem LLM und euren Systemen. Das ist gut für Governance und Sicherheit.
- Q&A öffnen.
- Demo-Repo oder Slides-Link zum Nachschlagen kommunizieren.
