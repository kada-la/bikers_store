using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Infrastructure.Persistence;
using SalesManagementSystem.Infrastructure;
using SalesManagementSystem.Application.Interfaces;
using SalesManagementSystem.Application;

// 1. Load environment variables from .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Register Infrastructure Services
builder.Services.AddInfrastructureServices(builder.Configuration);

// 4. Register Application Services (Scoped lifetime)
builder.Services.AddApplicationServices();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
