using Microsoft.EntityFrameworkCore;
using TodoApi.Configuration;
using TodoApi.Data;
using TodoApi.Middleware;
using TodoApi.Services;


var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddDbContext<TodoDbContext>(options =>
{
    options.UseSqlite(connectionString);
});
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
app.Use(async (context, next) =>
{
    Console.WriteLine($"CONSOLE LOGGERR: entering inline middlware");
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    await next();
    stopwatch.Stop();
    Console.WriteLine($"CONSOLE LOGGERR: [Middleware] {context.Request.Method} {context.Request.Path} executed in {stopwatch.ElapsedMilliseconds} ms");
    Console.WriteLine("CONSOLE LOGGERR: leaving inline middlware");
});
app.UseMiddleware<ApiKeyValidationMiddleware>();
app.MapControllers();
app.Run();
