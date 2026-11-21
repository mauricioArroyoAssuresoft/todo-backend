
using System.Net;
using System.Text.Json;

namespace TodoApi.Middleware
{
  public class GlobalExceptionMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
      _next = next;
      _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      try
      {
        await _next(context);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Unhandled exception ocurred for {Method} {Path}", context.Request.Method, context.Request.Path);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        var errorResponse = new
        {
          StatusCode = context.Response.StatusCode,
          Message = "An unexpected internal server error ocurred."
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
      }
    }
  }
}
