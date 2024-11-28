namespace UnistreamDemo.WebApi.Middleware
{
    using System.Net;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger)
        {
            app.UseExceptionHandler((System.Action<IApplicationBuilder>)(appError =>
            {
                appError.Run((RequestDelegate)(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        //logger.LogError($"Error: {contextFeature.Error}");
                        logger.LogError(contextFeature.Error, "Error while processing request from {Address}", context.Request.Path);

                        //TODO: Possible use Nuget Hellang.Middleware.ProblemDetails or ProblemDetailsFactory.CreateProblemDetails()
                        await context.Response.WriteAsync(new ProblemDetails()
                        {
                            Type = contextFeature.Error.GetType().Name,
                            Title = "Error",
                            Status = context.Response.StatusCode,
                            Detail = $"Internal Server Error: {contextFeature.Error}",
                            Instance = context.Request.Path
                        }.ToString());
                    }
                }));
            }));
        }
    }
}
