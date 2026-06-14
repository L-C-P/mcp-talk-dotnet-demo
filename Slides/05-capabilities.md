# Slide 05 – Capabilities: Tools, Resources, Prompts

## Slide text (EN)

### Three primitives – three distinct concerns

| Primitive | Purpose | StarAgent example |
|---|---|---|
| **Tool** | Executable function · model calls it, host runs it | `get_chart_position` – where is a song on the charts? |
| **Tool** | Action with a side effect | `book_venue` – reserve a concert venue |
| **Resource** | Read-only data, addressable by URI | `rider://artist/van-halen` – Van Halen's backstage requirements |
| **Prompt** | Reusable prompt template with placeholders | `concert_press_release` – generate a dramatic tour announcement |

### Decision guide

- **Tool** → the model needs to *do* something or fetch dynamic data
- **Resource** → stable, readable document or data set (like a file or config)
- **Prompt** → standardized, repeatable workflow the model should follow

---

## Speaker notes (DE)

- Die Semantik der drei Primitive präzise machen: Tool = execute, Resource = read, Prompt = orchestrate.
- StarAgent-Beispiele direkt zeigen: Wir bauen gleich alle drei live.
- Rider erklären: Ein Rider ist das echte Dokument, das jeder Künstler vor einem Konzert einreicht – Bühnenanforderungen, Catering, Sonderwünsche. Van Halens berühmteste Forderung: „Absolutely NO brown M&Ms." Das ist ein perfektes Beispiel für eine Resource – stabil, adressierbar, read-only.
- Enterprise-Brücke: Statt Rider → euer Konfigurations-Dokument, euer OpenAPI-Spec, euer Feature-Spec. Das Prinzip ist identisch.
- Governance-Hinweis: Die drei Primitiv-Typen haben unterschiedliche Risikoprofile – Tools können Seiteneffekte haben, Resources und Prompts sind read-only.
