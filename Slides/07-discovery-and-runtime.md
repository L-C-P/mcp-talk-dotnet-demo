# Slide 07 – Discovery & Runtime Sequence

## Slide text (EN)

### MCP lifecycle calls (for reference)

| Phase | Call | Direction |
|---|---|---|
| Setup | `initialize` + `notifications/initialized` | Client → Server |
| Discovery | `tools/list` · `resources/list` · `prompts/list` | Client → Server |
| Invocation | `tools/call` · `resources/read` · `prompts/get` | Client → Server |

### End-to-end runtime flow

```mermaid
sequenceDiagram
    actor User
    participant Host
    participant LLM
    participant MCP as MCP Server
    User->>Host: "Where does Bohemian Rhapsody<br/>rank on the charts?"
    Host->>LLM: User message + available tool definitions
    LLM-->>Host: Tool call request: get_chart_position(...)
    Host->>Host: Policy check / optional user approval
    Host->>MCP: tools/call · get_chart_position
    MCP-->>Host: { rank: 1, peak: 1, weeks: 52 }
    Host->>LLM: Tool result as new context
    LLM-->>Host: Final answer
    Host-->>User: "Bohemian Rhapsody is #1 – as always."
```

---

## Speaker notes (DE)

- Lifecycle-Tabelle nur kurz streifen: Diese Calls gibt es, der Host verwaltet sie automatisch. Man muss sie nicht selbst implementieren – das SDK erledigt das.
- Sequenzdiagramm ist das Herzstück: Hier wird sichtbar, dass der Host die Kontrolle behält. Das LLM macht einen Vorschlag (Tool Call), aber der Host entscheidet, ob er ausgeführt wird.
- Wichtige Botschaft: Der Host ist die Sicherheitsinstanz – nicht das LLM.
- Punchline zum Diagramm: Das letzte Ergebnis – „Bohemian Rhapsody ist #1 – wie immer" – ist unser Mock-Verhalten für die Demo. Kommt gleich live.
