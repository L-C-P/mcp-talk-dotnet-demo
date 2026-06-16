using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

FunctionsApplicationBuilder builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
       .AddApplicationInsightsTelemetryWorkerService()
       .ConfigureFunctionsApplicationInsights();

builder
   .ConfigureMcpTool("get_chart_position")
   .WithProperty("songTitle", McpToolPropertyType.String, "Song title", true)
   .WithProperty("artist", McpToolPropertyType.String, "Artist name", true)
   .WithProperty("chart", McpToolPropertyType.String, "Chart name");

builder
   .ConfigureMcpTool("book_venue")
   .WithProperty("artist", McpToolPropertyType.String, "Artist name", true)
   .WithProperty("city", McpToolPropertyType.String, "City", true)
   .WithProperty("date", McpToolPropertyType.String, "Date (yyyy-MM-dd)", true)
   .WithProperty("capacity", McpToolPropertyType.Integer, "Required capacity", true);

builder
   .ConfigureMcpPrompt("concert_press_release")
   .WithArgument("artist", "Artist name", true)
   .WithArgument("venue", "Venue name", true)
   .WithArgument("date", "Concert date", true)
   .WithArgument("tourName", "Tour name", true);

await builder.Build().RunAsync();
