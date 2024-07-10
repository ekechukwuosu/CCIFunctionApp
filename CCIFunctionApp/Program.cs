using CCIFunctionApp;
using CCIFunctionApp.Middleware;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using System.Threading.RateLimiting;

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
        services.AddHttpClient("my-client")
        .AddResilienceHandler("my-pipeline", builder =>
        {
            builder.AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = 4,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential
            });
            builder.AddRateLimiter(new HttpRateLimiterStrategyOptions
            {
                DefaultRateLimiterOptions = { PermitLimit = 4, QueueLimit = 2, QueueProcessingOrder = QueueProcessingOrder.OldestFirst },
            });
            builder.AddTimeout(TimeSpan.FromSeconds(5));
        });
    })
    .Build();

host.Run();