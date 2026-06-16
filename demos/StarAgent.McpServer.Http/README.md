# StarAgent.McpServer.Http

StarAgent MCP demo server over Streamable HTTP.

## Transport endpoint
- MCP endpoint: `http://localhost:3001/mcp`
- Health/info endpoint: `http://localhost:3001/`

## Shared capability model

Tools, resources, prompts, models, and services are reused from:
- `demos/StarAgent.McpServer.Shared`

Registered capabilities:
- Tool: `get_chart_position`
- Tool: `book_venue`
- Resource: `rider://artist/{name}`
- Prompt: `concert_press_release`

## Local run

```shell
dotnet run --project demos/StarAgent.McpServer.Http
```

## MCP host registration example

```json
{
  "servers": {
    "StarAgentHttp": {
      "type": "http",
      "url": "http://localhost:3001/mcp"
    }
  }
}
```
