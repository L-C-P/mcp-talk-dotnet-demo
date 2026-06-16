# Slide 06 – How LLM and MCP Actually Talk

## Slide text (EN)

### The LLM never sees MCP – the host is the translator

The **host** is embedded in the client application (Claude Code, Warp, GitHub Copilot, …).
It knows two languages: **MCP** on one side, and the **LLM's native format** on the other.

```
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

---

### It's just text – structured text

LLM and MCP server never talk directly. The **host** bridges both sides.

**Step 1 – Host discovers tools from MCP server (JSON-RPC):**
```json
// Request
{ "jsonrpc": "2.0", "id": 1, "method": "tools/list", "params": {} }

// Response
{
  "jsonrpc": "2.0", "id": 1,
  "result": {
    "tools": [{
      "name": "get_chart_position",
      "description": "Returns the chart position of a song.",
      "inputSchema": {
        "type": "object",
        "properties": {
          "song_title": { "type": "string" },
          "artist":     { "type": "string" },
          "chart":      { "type": "string", "default": "Billboard Hot 100" }
        },
        "required": ["song_title", "artist"]
      }
    }]
  }
}
```

**Step 2 – Host passes tool schema to LLM as a callable function (OpenAI-style):**
```json
{
  "type": "function",
  "function": {
    "name": "get_chart_position",
    "description": "Returns the chart position of a song.",
    "parameters": {
      "type": "object",
      "properties": {
        "song_title": { "type": "string" },
        "artist":     { "type": "string" },
        "chart":      { "type": "string" }
      },
      "required": ["song_title", "artist"]
    }
  }
}
```

**Step 3 – LLM responds with a tool call (just JSON in the chat response):**
```json
{ "name": "get_chart_position",
  "arguments": { "song_title": "Bohemian Rhapsody", "artist": "Queen" } }
```

**Step 4 – Host sends `tools/call` to MCP server:**
```json
{ "jsonrpc": "2.0", "id": 2, "method": "tools/call",
  "params": { "name": "get_chart_position",
               "arguments": { "song_title": "Bohemian Rhapsody", "artist": "Queen" } } }
```

**Step 5 – MCP server returns result → host feeds it back to LLM:**
```json
{ "jsonrpc": "2.0", "id": 2,
  "result": { "content": [{ "type": "text",
    "text": "{\"rank\":1,\"peak\":1,\"weeks\":52,\"chart\":\"Billboard Hot 100\"}" }] } }
```

---

## Speaker notes (DE)

- Kernbotschaft deutlich machen: LLM und MCP-Server sprechen **nie direkt** miteinander. Der Host ist immer der Vermittler und die Kontrollinstanz.
- JSON-RPC 2.0 ist kein Hexenwerk – es sind strukturierte Textnachrichten mit `method`, `params` und `result`. Genau das, was wir alle aus REST-APIs kennen, nur als bidirektionales Protokoll.
- Schritt 1–2 ist der Discovery-Vorgang: Der Host fragt den Server nach seinen Fähigkeiten und übersetzt das in Function-Definitions für das Modell.
- Schritt 3: Das LLM entscheidet, ob es ein Tool aufrufen will – es gibt einfach JSON zurück. Keine Magie.
- Schritt 4–5: Host führt den Tool-Call aus (policy check, ggf. user approval) und schickt das Ergebnis als neuen Context an das Modell.
- Highlight: Das LLM „sieht" nur die Tool-Schemata – es weiß nicht, ob dahinter .NET, Python oder ein Toaster steckt.
