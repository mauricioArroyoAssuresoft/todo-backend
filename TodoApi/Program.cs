using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TodoApi.Configuration;
using TodoApi.Data;
using TodoApi.Middleware;
using TodoApi.Services;


var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateLifetime = true,
//         ValidateIssuerSigningKey = true,
//         ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
//         ValidAudience = builder.Configuration["JwtSettings:Audience"],
//         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
//     };
// });
// builder.Services.AddAuthorization();
builder.Services.AddDbContext<TodoDbContext>(options =>
{
    options.UseSqlite(connectionString);
});
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
app.UseMiddleware<GlobalExceptionMiddleware>();
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
// app.UseAuthentication();
// app.UseAuthorization();
app.MapControllers();
app.Run();
