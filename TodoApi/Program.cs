using TodoApi.Middleware;
using TodoApi.Services;


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.Use(async (context, next) =>
{
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    await next();
    stopwatch.Stop();
    Console.WriteLine($"[Middleware] {context.Request.Method} {context.Request.Path} executed in {stopwatch.ElapsedMilliseconds} ms");
});
app.UseMiddleware<ApiKeyValidationMiddleware>();
app.Run();
