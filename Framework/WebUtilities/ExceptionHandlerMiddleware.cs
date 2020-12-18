using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Framework.WebUtilities
{
	/// <summary>
	/// Middleware for when exception is thrown in application
	/// </summary>
    public class ExceptionHandlerMiddleware
    {
        // The middleware delegate to call after this one finishes processing
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
			var ex = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
			httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

			// Return header to search through logs
			httpContext.Response.Headers.Add("TraceIdentifier", httpContext.TraceIdentifier);

			// Log exception
			var id = httpContext.User.Identity;
			Log.Error($"TraceIdentifier: {httpContext.TraceIdentifier}\n"
				+ $"Request URI: {httpContext.Request?.Method} {httpContext.Request?.GetEncodedUrl()}\n"
				+ $"Request Body: {Environment.GetEnvironmentVariable(Constants.PGRequestBody)}\n"
				+ $"Status Code: {httpContext.Response.StatusCode}\n"
				+ $"Request user: {id?.Name ?? "(anon)"}\n");

			// Output exception detail
			if (ex != null)
			{
				object shapeException(Exception e)
				{
					if (e == null) { return null; }
					return new
					{
						Type = e.GetType().FullName,
						e.Source,
						e.Message,
						e.StackTrace,
						InnerException = shapeException(e.InnerException)
					};
				}
				httpContext.Response.ContentType = "application/json";
				await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(shapeException(ex)));
			}
        }
    }
}
