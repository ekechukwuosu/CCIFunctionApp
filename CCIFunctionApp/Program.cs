using CCIFunctionApp;
using CCIFunctionApp.Middleware;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(app =>
    {
        app.UseMiddleware<CCIFunctionMiddleware>();
    })   
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddMemoryCache();
        services.AddSingleton<CacheService>();
    })
    .Build();

host.Run();