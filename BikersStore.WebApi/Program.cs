using DotNetEnv;
using SalesManagementSystem.Infrastructure;
using SalesManagementSystem.Application;

// 1. Load environment variables from .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// 2. Register Infrastructure Services
builder.Services.AddInfrastructureServices(builder.Configuration);

// 3. Register Application Services (Scoped lifetime)
builder.Services.AddApplicationServices();

// Add services to the container.
builder.Services.AddControllers();

// Add API Explorer metadata to help map controller endpoints for OpenAPI
builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(); // .NET 10 built-in OpenAPI support

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Serve the json document
}
else
{
    app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(new
            {
                title = "It's not you, it's us.",
                status = StatusCodes.Status500InternalServerError,
                detail = "Try again later."
            });
        });
    });
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
