using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CCIFunctionApp
{
    public class HttpTriggerFunction
    {
        private readonly ILogger<HttpTriggerFunction> _logger;
        private readonly CacheService _cacheService;
        public HttpTriggerFunction(ILogger<HttpTriggerFunction> logger, CacheService cacheService)
        {
            _logger = logger;
            _cacheService = cacheService;
        }

        [Function("HttpTriggerFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var key = "key";
            if (req.Method == HttpMethod.Post.Method)
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);
                if (data != null && data.ContainsKey("key"))
                {
                    var cacheValue = data["key"];
                    _cacheService.GetOrCreate(key, entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                        return cacheValue;
                    });
                }
                else
                {
                    return new BadRequestObjectResult($"Bad Request: 'value' is not found in the request body");
                }
            }
            else if (req.Method == HttpMethod.Get.Method)
            {
                var queryParams = System.Web.HttpUtility.ParseQueryString(req.QueryString.Value);
                var cacheKey = queryParams["key"].ToString();

                if (string.IsNullOrEmpty(cacheKey))
                {
                    return new BadRequestObjectResult($"Bad Request: 'key' query parameter is required.");
                }

                var cachedValue = _cacheService.GetOrCreate(cacheKey, entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    return "Default value";
                });
                return new OkObjectResult($"Cache Value for '{cacheKey}': {cachedValue}");
            }
            return new OkObjectResult($"Request processed.");
        }
    }
}
