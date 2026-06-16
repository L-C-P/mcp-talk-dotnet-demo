# StarAgent.McpServer.Stdio

StarAgent demo MCP server for the talk.  
Transport: `stdio` (local host-launched process).

Reusable capabilities (tools/resources/prompts + domain models/services) are provided by:
- `demos/StarAgent.McpServer.Shared`

## Implemented capabilities

### Tools
- `get_chart_position`
  - Input: `songTitle`, `artist`, optional `chart`
  - Output: chart rank/peak/weeks + metadata
- `book_venue`
  - Input: `artist`, `city`, `date` (`yyyy-MM-dd`), `capacity`
  - Output: booking status (`confirmed`, `waitlist`, `rejected`) and details

### Resource
- `rider://artist/{name}`
  - Returns backstage rider JSON
  - Includes the demo punchline for `van-halen` (`Absolutely NO brown M&Ms`)

### Prompt
- `concert_press_release`
  - Input: `artist`, `venue`, `date`, `tourName`
  - Returns one user message prompt template for a dramatic press release

## Local development

Run the server from source:

```shell
dotnet run --project demos/StarAgent.McpServer.Stdio
```

Example MCP host registration:

```json
{
  "servers": {
    "StarAgent": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/absolute/path/to/blm2026/demos/StarAgent.McpServer.Stdio"
      ]
    }
  }
}
```

## Packaging

Build NuGet package:

```shell
dotnet pack demos/StarAgent.McpServer.Stdio -c Release
```