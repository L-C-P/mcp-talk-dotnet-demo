# Slide 11 – MCP on Azure Functions

## Slide text (EN)

### Hosting an MCP server serverless – Azure Functions (isolated worker)

**NuGet packages:**
```shell
dotnet add package ModelContextProtocol --prerelease
dotnet add package Microsoft.Azure.Functions.Worker
dotnet add package Microsoft.Azure.Functions.Worker.Extensions.Http
```

**`Program.cs` – DI registration:**
```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddMcpServer()
            .WithToolsFromAssembly()
            .WithResourcesFromAssembly()
            .WithPromptsFromAssembly();
    })
    .Build();

await host.RunAsync();
```

**`McpFunction.cs` – HTTP trigger routes to MCP handler:**
```csharp
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;

public class McpFunction(IServiceProvider services)
{
    [Function("mcp")]
    public async Task<HttpResponseData> HandleAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "mcp")]
        HttpRequestData req)
    {
        // Forward the HTTP request into the MCP middleware pipeline
        var handler = services.GetRequiredService<McpServerHttpHandler>();
        return await handler.HandleAsync(req);
    }
}
```

### Why Functions fit MCP well

- `Stateless = true` aligns perfectly with the consumption plan model
- Scale-to-zero – pay only when tools are called
- Auth via Function keys or Azure AD out of the box
- Cold start: keep server logic lean, data files embedded as resources

---

## Speaker notes (DE)

- Azure Functions isolated worker ist das moderne Modell (.NET 8 / .NET 10) – nicht den älteren in-process-Host verwenden.
- Die Tool-/Resource-/Prompt-Klassen bleiben identisch zu stdio und ASP.NET Core – nur der Host-Wrapper ändert sich.
- `McpServerHttpHandler` ist die abstrakte Komponente, die HTTP-Request/-Response in das MCP-Protokoll übersetzt. Exakter API-Name kann je nach SDK-Version variieren – vor dem Event prüfen.
- Stateless-Transport-Hinweis: Azure Functions sind zustandslos – das passt direkt zu `Stateless = true` im MCP HTTP-Transport.
- Produktionsrelevanz: Viele Enterprise-Teams, die bereits Functions nutzen, können so MCP-Fähigkeiten mit minimaler Infrastruktur exposieren.
- Auth: Function-Keys für einfache Szenarien, Azure AD für Unternehmensumgebungen.
