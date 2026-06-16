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

If this repository is published with static hosting (for example GitHub Pages), the `slidev/dist` folder can be used as the deploy target.
For GitHub Pages deployments where `slidev/dist` is published as the site root (recommended), use base path `/mcp-talk-dotnet-demo/`.
Use `/mcp-talk-dotnet-demo/slidev/dist/` only if you publish the repository tree itself and keep `slidev/dist` in the served URL.

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
