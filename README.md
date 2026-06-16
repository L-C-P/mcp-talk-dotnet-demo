# BLM 2026 – MCP Talk Repository

This repository contains the presentation materials and implementation workspace for the **“Behind the Scenes: MCP”** talk.

## Content
- `slidev/` – Slidev presentation project (main source and build output)
- `slides/` – original slide source files (one topic per file)
- `demos/` – demo project workspace for MCP implementations
- `StarAgent-MCP-Talk.md` / `mcp-dotnet-talk-concept.md` – concept and talk notes

## Presentation
- Slide source: `slidev/slides.md`
- Built output folder: [`slidev/dist`](slidev/dist)
- Presentation entry point: [`slidev/dist/index.html`](slidev/dist/index.html)
- Presentation URL (repo-based GitHub Pages): [https://l-c-p.github.io/mcp-talk-dotnet-demo/slidev/dist/](https://l-c-p.github.io/mcp-talk-dotnet-demo/slidev/dist/)

The default Slidev build in `slidev/package.json` is configured for this path (`/mcp-talk-dotnet-demo/slidev/dist/`), so the presentation can be opened directly from the `slidev/dist` folder URL.

## Local setup
1. Install dependencies:
   - `cd slidev`
   - `npm install`
2. Start presentation in dev mode:
   - `npm run dev`
3. Build static output:
   - `npm run build`

## Demo projects
The demo workspace is prepared in `demos/` and includes:
- `Demos.slnx` – solution containing all demo projects
- `StarAgent.McpServer.Shared` – shared capabilities used by all host variants
- `StarAgent.McpServer.Stdio` – local MCP server over stdio
- `StarAgent.McpServer.Http` – MCP server over Streamable HTTP
- `StarAgent.McpServer.AzureFunctions` – serverless MCP host on Azure Functions

Each folder includes a README with scope and next implementation steps.

## License
- Source code: Apache License 2.0 (`LICENSE`)
- Documentation and slides: Creative Commons Attribution 4.0 (`LICENSE-docs`)

Attribution must be preserved as stated in `NOTICE` (Denis Sowa).
