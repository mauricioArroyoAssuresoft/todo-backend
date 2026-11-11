
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
      Console.WriteLine($"CONSOLE LOGGERR: entering api key middlware");
      if (!context.Request.Headers.ContainsKey("X-App-Client"))
      {
        Console.WriteLine("CONSOLE LOGGERR: the api key middleware short-circuited the request");
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("missing required x-app-client header");
        return;
      }

      await _next(context);
      Console.WriteLine($"CONSOLE LOGGERR: leaving api key middlware");
    }

  }
}
