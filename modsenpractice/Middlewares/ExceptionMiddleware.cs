using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace modsenpractice.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                if (context.Response.StatusCode < 200 || context.Response.StatusCode >= 300)
                {
                    await HandleNonSuccessResponseAsync(context);
                }
            }
            catch (Exception ex)
            {
                await HandleCustomExceptionResponseAsync(context, ex);
            }
        }

        private async Task HandleCustomExceptionResponseAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;

            context.Response.StatusCode = ex switch
            {
                ArgumentException or ArgumentNullException => (int)HttpStatusCode.BadRequest,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                InvalidOperationException => (int)HttpStatusCode.BadRequest,
                NotImplementedException => (int)HttpStatusCode.NotImplemented,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = GetMessageForStatusCode(context.Response.StatusCode),
                Detailed = ex.Message
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }

        private async Task HandleNonSuccessResponseAsync(HttpContext context)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = GetMessageForStatusCode(context.Response.StatusCode)
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }

        private string GetMessageForStatusCode(int statusCode) => statusCode switch
        {
            400 => "Bad Request - The request was invalid or cannot be served.",
            401 => "Unauthorized - Authentication is required and has failed or has not been provided.",
            403 => "Forbidden - The request was valid, but the server is refusing action.",
            404 => "Not Found - The requested resource could not be found.",
            500 => "Internal Server Error - An unexpected condition was encountered.",
            501 => "Not Implemented - The server does not support the functionality required.",
            _ => $"Request failed with status code {statusCode}."
        };
    }
}