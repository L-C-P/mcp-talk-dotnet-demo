using StarAgent.McpServer.Shared.Prompts;
using StarAgent.McpServer.Shared.Resources;
using StarAgent.McpServer.Shared.Tools;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddMcpServer()
       .WithHttpTransport()
       .WithToolsFromAssembly(typeof(ChartTools).Assembly)
       .WithResourcesFromAssembly(typeof(RiderResources).Assembly)
       .WithPromptsFromAssembly(typeof(PressReleasePrompts).Assembly);

WebApplication app = builder.Build();

app.UseStaticFiles();
app.MapGet("/", () => "StarAgent MCP HTTP demo server is running.");
app.MapMcp("/mcp");

await app.RunAsync("http://localhost:3001");
