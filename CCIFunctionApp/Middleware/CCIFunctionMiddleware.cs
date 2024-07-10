using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using System.Net;

namespace CCIFunctionApp.Middleware
{
    public class CCIFunctionMiddleware : IFunctionsWorkerMiddleware
    {
        async Task IFunctionsWorkerMiddleware.Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var request = await context.GetHttpRequestDataAsync();

            if (request != null)
            {
                if (!request.Headers.TryGetValues("X-CCi-User-Groups", out var headerValues) || string.IsNullOrEmpty(headerValues.FirstOrDefault()))
                {
                    var response = request.CreateResponse(HttpStatusCode.Unauthorized);
                    await response.WriteStringAsync("Unauthorized: Missing or invalid 'X-CCi-User-Groups' header.");

                    context.GetInvocationResult().Value = response;

                    return;
                }
            }

            await next(context);
        }
    }
}
