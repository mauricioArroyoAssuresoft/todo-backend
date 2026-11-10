
namespace TodoApi.Middleware
{
  public class ApiKeyValidationMiddleware
  {
    private readonly RequestDelegate _next;

    public ApiKeyValidationMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      if (!context.Request.Headers.ContainsKey("X-App-Client"))
      {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("missing required x-app-client header");
        return;
      }

      await _next(context);
    }

  }
}
