# StarAgent.McpServer.AzureFunctions

StarAgent MCP demo serverless host on Azure Functions (isolated worker model).

## Implemented capabilities
- MCP tool trigger: `get_chart_position`
- MCP tool trigger: `book_venue`
- MCP resource trigger: `rider://artist/{name}`
- MCP prompt trigger: `concert_press_release`

## Discovery endpoints
- `/.well-known/mcp.json`
- `/.well-known/mcp/server.json`

## Local run

```shell
func start
```

MCP endpoint (Functions runtime):
- `http://localhost:7071/runtime/webhooks/mcp`